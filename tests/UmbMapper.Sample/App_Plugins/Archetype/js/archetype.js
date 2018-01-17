/* Version 1.18.0 */
angular.module("umbraco").controller("Imulus.ArchetypeController", function ($scope, $http, $filter, $interpolate, assetsService, angularHelper, notificationsService, $timeout, fileManager, entityResource, archetypeService, archetypeLabelService, archetypeCacheService, archetypePropertyEditorResource, archetypeLocalizationService) {

    // Variables.
    var draggedParent;
    
    var isCacheInitialized = false;

    //$scope.model.value = "";
    $scope.model.hideLabel = $scope.model.config.hideLabel == 1;

    //get a reference to the current form
    $scope.form = $scope.form || angularHelper.getCurrentForm($scope);

    //set the config equal to our prevalue config
    $scope.model.config = $scope.model.config.archetypeConfig;

    //ini the model
    $scope.model.value = $scope.model.value || getDefaultModel($scope.model.config);

    // store the umbraco property alias to help generate unique IDs.  Hopefully there's a better way to get this in the future :)
    $scope.umbracoHostPropertyAlias = $scope.$parent.$parent.model.alias;
    
    $scope.isDebuggingEnabled = Umbraco.Sys.ServerVariables.isDebuggingEnabled;

    $scope.overlayMenu = {
        show: false,
        style: {}
    };

    init();
    
    //helper to get $eval the labelTemplate
    $scope.fieldsetTitles = [];
    $scope.getFieldsetTitle = function (fieldsetConfigModel, fieldsetIndex) {
       
        if(!isCacheInitialized) {         
            return "";
        }
    
        // Ensure the collection of titles is large enough.
        ensureEnoughTitles(fieldsetIndex + 1);
        var title = $scope.fieldsetTitles[fieldsetIndex];

        // Return the title if it's already been loaded.
        if (title.loaded) {
            return title.value;
        }

        // Return early if the title is still loading.
        if (title.loading) {
            return;
        }

        // Start loading the title.
        title.loading = true;
        title.loaded = false;
        title.value = null;

        archetypeLabelService.getFieldsetTitle($scope, fieldsetConfigModel, fieldsetIndex).then(function(value) {
            // Finished loading the title.
            title.loaded = true;
            title.loading = false;
            title.value = value;
        });
    };

    /**
     * Ensure the collection of fieldset titles is large enough to accommodate the number of fieldsets.
     * @param count The number of fieldsets.
     */
    function ensureEnoughTitles(count) {
        while ($scope.fieldsetTitles.length < count) {
            $scope.fieldsetTitles.push({
                loading: false,
                loaded: false,
                value: null
            });
        }
    }

    //sort config
    $scope.sortableOptions = {
        axis: 'y',
        cursor: "move",
        handle: ".handle",
        tolerance: "pointer",
        activate: function(ev, ui) {

            // Variables.
            var parentItem = ui.item.parent();
            var thisElement = parentItem[0];
            var targetElement = ev.target;
            var targetItem = angular.element(targetElement);

            // The source of the drag can always be dropped back onto.
            if (targetElement === thisElement) {
                return;
            }

            // Variables.
            var sourceScope = ui.item.scope();
            var targetScope = targetItem.scope();
            var valid = canMove(sourceScope, targetScope);

            // If the sortable can't be moved to the target Archetype, disable
            // the target Archetype's sortable temporarily.
            if (!valid) {
                var targetSortable = getSortableWidgetInstance(targetItem);
                targetSortable.disable();
                parentItem.sortable("refresh");
                archetypeService.rememberDisabledSortable(targetSortable);
            }

        },
        start: function(ev, ui) {
            archetypeService.getEditors().addClass('archetypeDragging');
            archetypeService.storeEditors(ui.item.parent());
            $scope.$apply(function() {
                draggedParent = ui.item.parent();
                draggedParent.scope().doingSort = true;
            });
        },
        update: function (ev, ui) {

            // Variables.
            var targetScope = ui.item.sortable.droptarget.scope();
            var sourceScope = ui.item.scope();
            var sameScope = sourceScope === targetScope;
            var sourceIndex = ui.item.sortable.index;

            // Special constraints for when moving between Archetypes.
            // If sourceScope is populated, we are in the first of the two updates (when
            // moving between lists, ui-sortable calls the update function twice).
            if (sourceScope && !sameScope) {

                // Variables.
                var valid = canMove(sourceScope, targetScope);

                // If update isn't allowed, cancel the drag operation.
                if (!valid) {
                    ui.item.sortable.cancel();
                    return;
                }

                // Clear the validations for this item.
                clearValidations(ui.item.parent());
                clearValidations(ui.item.sortable.droptarget);

                // Reset "isValid" on the properties and fieldsets.
                var fieldsetGroups = [
                    $scope.model.value.fieldsets,
                    targetScope.model.value.fieldsets
                ];
                _.each(fieldsetGroups, function(fieldsets) {
                    recurseProperties(function(property, fieldset) {
                        property.isValid = true;
                        fieldset.isValid = true;
                    }, fieldsets);
                });

                // Move the activated fieldset to the target Archetype.
                var movedFieldset = $scope.model.value.fieldsets[sourceIndex];
                var loadedIndex = $scope.loadedFieldsets.indexOf(movedFieldset);
                if (loadedIndex >= 0) {
                    $scope.loadedFieldsets.splice(loadedIndex, 1);
                    if (targetScope.loadedFieldsets.indexOf(movedFieldset) < 0) {
                        targetScope.loadedFieldsets.push(movedFieldset);
                    }
                }

            }

            // Set scope dirty.
            $scope.setDirty();

        },
        stop: function (ev, ui) {
            // Done sorting.
            draggedParent.scope().doingSort = false;

            // Enable disabled sortables.
            archetypeService.enableSortables();

            // Restore rich text editors.
            var parent = null;
            var target = ui.item.sortable.droptarget;
            if (target) {
                parent = target.parent();
            }
            archetypeService.restoreEditors(parent);

            archetypeService.getEditors().removeClass('archetypeDragging');
        }
    };

    // Clears the Angular validations in an Archetype.
    function clearValidations(el) {
        var combined = $(".archetypeSortable", el).add(el);
        combined.each(function(index, item) {
            var $item = angular.element(item);
            var cont = $item.controller("ngModel");
            var err = cont.$error;
            var keys = Object.keys(err);
            for (var i = 0; i < keys.length; i++) {
                cont.$setValidity(keys[i], true);
            }
        });
    }

    // Enable cross-archetype dragging?
    if ($scope.model.config.enableCrossDragging) {
        $scope.sortableOptions.connectWith = ".archetypeSortable:not(.invalid)";
    }

    // Checks if the specified model's properties match all of the properties in any of
    // the specified fieldsets, while also checking if the fieldset aliases match.
    // This is an indicator of the compatibility of a fieldset model with a collection
    // of fieldset configurations.
    function modelMatchesAnyFieldset(model, fieldsets) {

        // Loop through fieldsets to find a match.
        for (var i = 0; i < fieldsets.length; i++) {
            var fieldset = fieldsets[i];

            // Confirm that this configured fielset contains exactly
            // the same properties as those that were supplied.
            var valid =
                // Does the alias match?
                model.alias === fieldset.alias &&
                // Does this fieldset have all the properties?
                arePropertiesSubset(model.properties, fieldset.properties) &&
                // Does the property array have all the fieldset properties?
                arePropertiesSubset(fieldset.properties, model.properties);

            // Match found?
            if (valid) {
                return true;
            }

        }

        // No match found.
        return false;

    }

    // Checks if the specified array of fieldsets contains the specified fieldset.
    function arrayContainsFieldset(fieldset, fieldsets) {
        for (var i = 0; i < fieldsets.length; i++) {
            if (fieldsets[i] === fieldset) {
                return true;
            }
        }
        return false;
    }

    // Confirms that an array of properties is a subset of another array of properties.
    function arePropertiesSubset(subset, superset) {

        // Loop through the subset of properties.
        for (var j = 0; j < subset.length; j++) {
            var subProperty = subset[j];
            var matchedProp = false;

            // Loop through the superset to find a matching property from the subset.
            for (var k = 0; k < superset.length; k++) {
                var superProperty = superset[k];
                if (superProperty.alias === subProperty.alias &&
                    superProperty.dataTypeGuid === subProperty.dataTypeGuid) {
                    matchedProp = true;
                    break;
                }
            }

            // If no matching property could be found, the array is not a subset.
            if (!matchedProp) {
                return false;
            }

        }

        // The array is a subset.
        return true;

    }

    //handles a fieldset add
    $scope.openFieldsetPicker = function ($index, event) {
        if ($scope.canAdd() == false) {
            return;
        }

        var allFieldsets = [];
        _.each($scope.model.config.fieldsets, function (fieldset) {
            var icon = fieldset.icon;
            allFieldsets.push({
                alias: fieldset.alias,
                label: fieldset.label,
                icon: (fieldset.icon || "icon-document-dashed-line"), // default icon if none is chosen
                group: fieldset.group ? fieldset.group.name : null,
                previewImage: fieldset.previewImage
            });
        });
        // sanity check
        if (allFieldsets == 0) {
            return;
        }
        if (allFieldsets.length == 1) {
            // only one fieldset type - no need to display the picker
            $scope.addRow(allFieldsets[0].alias, $index);
            return;
        }

        $scope.overlayMenu.fieldsetGroups = [];
        if ($scope.model.config.fieldsetGroups && $scope.model.config.fieldsetGroups.length > 0) {
            _.each($scope.model.config.fieldsetGroups, function (fieldsetGroup) {
                $scope.overlayMenu.fieldsetGroups.push({ name: fieldsetGroup.name, fieldsets: $filter("filter")(allFieldsets, { group: fieldsetGroup.name }, true) });
            })
        }
        else {
            $scope.overlayMenu.fieldsetGroups.push({ name: "", fieldsets: allFieldsets });
        }
        $scope.overlayMenu.index = $index;
        $scope.overlayMenu.activeFieldsetGroup = $scope.overlayMenu.fieldsetGroups[0];

        // calculate overlay position
        // - yeah... it's jQuery (ungh!) but that's how the Grid does it.
        var offset = $(event.target).offset();
        var scrollTop = $(event.target).closest(".umb-panel-body").scrollTop();
        if (offset.top < 400) {
            $scope.overlayMenu.style.top = 300 + scrollTop;
        }
        else {
            $scope.overlayMenu.style.top = offset.top - 150 + scrollTop;
        }
        $scope.overlayMenu.show = true;
    };

    $scope.closeFieldsetPicker = function () {
        $scope.overlayMenu.show = false;
        $scope.overlayMenu.fieldsetPreview = null;
    };
    
    $scope.pickFieldset = function (fieldsetAlias, $index) {
        $scope.closeFieldsetPicker();
        $scope.addRow(fieldsetAlias, $index);
    };    

    $scope.openFieldsetPreview = function (fieldset) {
        $scope.overlayMenu.fieldsetPreview = fieldset;
    }

    $scope.closeFieldsetPreview = function () {
        $scope.overlayMenu.fieldsetPreview = null;
    }

    $scope.addRow = function (fieldsetAlias, $index) {
        if ($scope.canAdd()) {
            if ($scope.model.config.fieldsets) {
                var newFieldset = getEmptyRenderFieldset($scope.getConfigFieldsetByAlias(fieldsetAlias));

                if (typeof $index != 'undefined')
                {
                    $scope.model.value.fieldsets.splice($index + 1, 0, newFieldset);
                }
                else
                {
                    $scope.model.value.fieldsets.push(newFieldset);
                }
            }

            addCustomPropertiesToFieldset(newFieldset);

            $scope.setDirty();

            $scope.$broadcast("archetypeAddFieldset", {index: $index, visible: countVisible()});

            newFieldset.collapse = $scope.model.config.enableCollapsing ? true : false;

            // If the fieldset is not collapsed, it should be instantly loaded.
            if (!newFieldset.collapse) {
                $scope.loadedFieldsets.push(newFieldset);
            }
            
            $scope.focusFieldset(newFieldset);
            $scope.handleMandatoryValidation();
        }
    }

    $scope.removeRow = function ($index) {
        if ($scope.canRemove()) {
            if (confirm('Are you sure you want to remove this?')) {
                $scope.setDirty();
                $scope.model.value.fieldsets.splice($index, 1);
                $scope.$broadcast("archetypeRemoveFieldset", {index: $index});
                $scope.handleMandatoryValidation();
            }
        }
    }

    $scope.cloneRow = function ($index) {
        if ($scope.canClone() && typeof $index != 'undefined') {
            var newFieldset = angular.copy($scope.model.value.fieldsets[$index]);

            if(newFieldset) {

                // Regenerate the temporary ID on each nested fieldset.
                // This is done because no two fieldsets should have the same
                // temporary ID.
                recurseProperties(function(property) {
                    archetypeService.ensureTemporaryId(property, true);
                }, [newFieldset]);

                $scope.model.value.fieldsets.splice($index + 1, 0, newFieldset);

                $scope.setDirty();

                newFieldset.collapse = $scope.model.config.enableCollapsing ? true : false;

                // If the fieldset is not collapsed, it should be instantly loaded.
                if (!newFieldset.collapse) {
                    $scope.loadedFieldsets.push(newFieldset);
                }

                $scope.focusFieldset(newFieldset);
            }
        }
    }

    $scope.enableDisable = function (fieldset) {
        fieldset.disabled = !fieldset.disabled;
        // explicitly set the form as dirty when manipulating the enabled/disabled state of a fieldset
        $scope.setDirty();
        $scope.handleMandatoryValidation();
    }

    //helpers for determining if a user can do something
    $scope.canAdd = function () {
        if ($scope.model.config.maxFieldsets)
        {
            var visibleCount = countVisible();
            var maxFieldsets = $scope.model.config.maxFieldsets;
            return visibleCount < maxFieldsets;
        }

        return true;
    };

    //helper that returns if an item can be removed
    $scope.canRemove = function () {
        return countVisible() > 1 
            || ($scope.model.config.maxFieldsets == 1 && $scope.model.config.fieldsets.length > 1)
            || $scope.model.config.startWithAddButton;
    };

    $scope.canClone = function () {

        if (!$scope.model.config.enableCloning) {
            return false;
        }

        if ($scope.model.config.maxFieldsets)
        {
            return countVisible() < $scope.model.config.maxFieldsets;
        }

        return true;
    }

    //helper that returns if an item can be sorted
    $scope.canSort = function ()
    {
        // Sorting can occur if there are multiple fieldsets, or if there is only one
        // fieldset that can be removed (in which case it can be sorted into an entirely
        // different Archetype).
        return countVisible() > 1 || $scope.canRemove();
    };

    //helper that returns if an item is the last and it's being sorted.
    $scope.sortingLastItem = function() {
        return $scope.doingSort && $scope.model.value.fieldsets.length <= 1;
    };

    //helper that returns if an item can be disabled
    $scope.canDisable = function () {
        return $scope.model.config.enableDisabling;
    }

    //helpers for determining if the add button should be shown
    $scope.showAddButton = function () {
        var visible = countVisible();
        return (visible === 0 && $scope.model.config.startWithAddButton) 
            || (visible > 0 && $scope.canAdd());
    }

    //helper for determining if no content is available yet
    $scope.showEmptyContentHint = function () {
        return $scope.model.config.startWithAddButton
            && countVisible() === 0;
            ///&& $scope.model.config.fieldsets.length == 1;
    }

    //helper that returns if an item can use publishing
    $scope.canPublish = function () {
        return $scope.model.config.enablePublishing;
    }

    $scope.canUseMemberGroups = function() {
        return $scope.model.config.enableMemberGroups;
    }

    //helper that returns if the "misc fieldset configuration" section should be visible
    $scope.canConfigure = function () {
        // currently the "misc fieldset configuration" section contains the publishing and the member groups setup
        return $scope.canPublish() || $scope.canUseMemberGroups();
    }

    $scope.showDisableIcon = function (fieldset) {
        if ($scope.canDisable() == false) {
            return false;
        }
        // disabled state takes precedence over publishing
        if (fieldset.disabled) {
            return true;
        }
        return $scope.isDisabledByPublishing(fieldset) == false;
    }

    $scope.showPublishingIcon = function (fieldset) {
        if ($scope.canPublish() == false) {
            return false;
        }
        if ($scope.canDisable()) {
            // disabled state takes precedence over publishing
            if (fieldset.disabled) {
                return false;
            }
            return $scope.isDisabledByPublishing(fieldset);
        }
        return true;
    }

    $scope.isDisabledByPublishing = function(fieldset) {
        if ($scope.canPublish() === false) {
            return false;
        }
        // NOTE: all comparison is done in local datetime
        //       - that's fine because the selected local datetimes will be converted to UTC datetimes when submitted
        if (fieldset.expireDateModel && fieldset.expireDateModel.value && (moment() > moment(fieldset.expireDateModel.value))) {
            // an expired release affects the fieldset
            return true;
        }
        if (fieldset.releaseDateModel && fieldset.releaseDateModel.value && (moment(fieldset.releaseDateModel.value) > moment())) {
            // a pending release affects the fieldset
            return true;
        }
        return false;
    }

    $scope.isDisabled = function(fieldset) {
        if (fieldset.disabled) {
            return true;
        }
        return $scope.isDisabledByPublishing(fieldset);
    }

    $scope.numberOfEnabledFieldsets = function () {
        if (!$scope.model.value.fieldsets) {
            return 0;
        }
        return _.filter($scope.model.value.fieldsets, function (f) {
            return $scope.isDisabled(f) == false;
        }).length;
    }

    //helper, ini the render model from the server (model.value)
    function init() {
        $scope.model.value = removeNulls($scope.model.value);
        addDefaultProperties($scope.model.value.fieldsets);
        
        archetypeCacheService.initialize().then(function() {
            isCacheInitialized = true;
        });

        $timeout(function () {
            $scope.handleMandatoryValidation();
        }, 50);

        // localize the message for minimum fieldsets validation errors
        archetypeLocalizationService.localize("minFieldsetsMessage").then(function (value) {
            value = value || "You must add minimum {{minFieldsets}} items";
            var exp = $interpolate(value);
            $scope.minFieldsetsMessage = exp({ minFieldsets: $scope.model.config.minFieldsets });
        });
    }

    function addDefaultProperties(fieldsets)
    {
        _.each(fieldsets, function (fieldset)
        {
            fieldset.collapse = false;
            fieldset.isValid = true;
        });
    }

    function addCustomProperties(fieldsets) {
        // make sure we have loaded moment.js before using it
        assetsService.loadJs("lib/moment/moment-with-locales.js").then(function() {
            _.each(fieldsets, function(fieldset) {
                addCustomPropertiesToFieldset(fieldset);
            });
        });
    }

    function addCustomPropertiesToFieldset(fieldset) {
        // create models for publish configuration (utilizing the built-in datepicker data type)
        // NOTE: all datetimes must be converted from UTC to local
        fieldset.releaseDateModel = {
            alias: _.uniqueId("archetypeReleaseDate_"),
            view: "datepicker",
            value: fromUtc(fieldset.releaseDate)
        };
        fieldset.expireDateModel = {
            alias: _.uniqueId("archetypeExpireDate_"),
            view: "datepicker",
            value: fromUtc(fieldset.expireDate)
        };
        // create model for allowed member groups
        fieldset.allowedMemberGroupsModel = {
            alias: _.uniqueId("archetypeAllowedMemberGroups_"),
            view: "membergrouppicker",
            value: fieldset.allowedMemberGroups
        };
    }

    //helper to get the correct fieldset from config
    $scope.getConfigFieldsetByAlias = function(alias) {
        return _.find($scope.model.config.fieldsets, function(fieldset){
            return fieldset.alias == alias;
        });
    }

    //helper to get a property by alias from a fieldset
    $scope.getPropertyValueByAlias = function(fieldset, propertyAlias) {
        var property = _.find(fieldset.properties, function(p) {
            return p.alias == propertyAlias;
        });
        return (typeof property !== 'undefined') ? property.value : '';
    };

    $scope.isCollapsed = function(fieldset)
    {
        if(typeof fieldset.collapse === "undefined")
        {
            fieldset.collapse = $scope.model.config.enableCollapsing ? true : false;
        }
        return fieldset.collapse;
    };

    // added to track loaded fieldsets 
    $scope.loadedFieldsets = [];
    $scope.isLoaded = function (fieldset) {
        return $scope.loadedFieldsets.indexOf(fieldset) >= 0;
    }

    //helper for expanding/collapsing fieldsets
    $scope.focusFieldset = function(fieldset){
        fixDisableSelection();

        if (!$scope.model.config.enableCollapsing) {
            return;
        }

        // collapse all other fieldsets if "multiple open fieldsets" is not enabled
        if(!$scope.model.config.enableMultipleOpen) 
        {
            _.each($scope.model.value.fieldsets, function (f) {
                if(f != fieldset)
                {
                    f.collapse = true;
                }
            });
        }

        if(!fieldset && $scope.model.value.fieldsets.length == 1)
        {
            $scope.model.value.fieldsets[0].collapse = false;
            $scope.loadedFieldsets.push($scope.model.value.fieldsets[0]);
            return;
        }

        if(fieldset)
        {
            fieldset.collapse = !fieldset.collapse;
            if($scope.loadedFieldsets.indexOf(fieldset) < 0)
            {
                $scope.loadedFieldsets.push(fieldset);
            }
        }
    }

    //ini the fieldset expand/collapse
    $scope.focusFieldset();

    // Fieldsets which cannot be collapsed should start expanded.
    _.each($scope.model.value.fieldsets, function(fieldset) {
        fieldset.collapse = $scope.model.config.enableCollapsing;
    });
    $scope.loadedFieldsets = _.where($scope.model.value.fieldsets, { collapse: false });

    //developerMode helpers
    $scope.model.value.toString = stringify;

    // issue 114: register handler for file selection
    $scope.model.value.setFiles = setFiles;

    //encapsulate stringify (should be built into browsers, not sure of IE support)
    function stringify() {
        return JSON.stringify(this);
    }

    // issue 114: handler for file selection
    function setFiles(files) {
        // get all currently selected files from file manager
        var currentFiles = fileManager.getFiles();
        
        // get the files already selected for this archetype (by alias)
        var archetypeFiles = [];
        _.each(currentFiles, function (item) {
            if (item.alias === $scope.model.alias) {
                archetypeFiles.push(item.file);
            }
        });

        // add the newly selected files
        _.each(files, function (file) {
            archetypeFiles.push(file);
        });

        // update the selected files for this archetype (by alias)
        fileManager.setFiles($scope.model.alias, archetypeFiles);
    }

    // Deep watch for changes.
    $scope.$watch('model.value', function () {

        // Empty titles. If the value has changed, the titles may have changed.
        $scope.fieldsetTitles = [];

    }, true);

    //watch for changes
    $scope.$watch('model.value', function (v) {
        if ($scope.model.config.developerMode) {
            console.log(v);
            if (typeof v === 'string') {
                $scope.model.value = JSON.parse(v);
                $scope.model.value.toString = stringify;
            }
        }

        // issue 114: re-register handler for files selection and reset the currently selected files on the file manager
        $scope.model.value.setFiles = setFiles;
        fileManager.setFiles($scope.model.alias, []);

        // reset submit watcher counter on save
        $scope.activeSubmitWatcher = 0;

        // init loaded fieldsets tracking
        _.each($scope.model.value.fieldsets, function (fieldset) {
            fieldset.collapse = $scope.model.config.enableCollapsing ? true : false;
        });
        $scope.loadedFieldsets = _.where($scope.model.value.fieldsets, { collapse: false });

        // create properties needed for the backoffice to work (data that is not serialized to DB)
        addCustomProperties($scope.model.value.fieldsets);
    });

    //helper to count what is visible
    function countVisible()
    {
        return $scope.model.value.fieldsets.length;
    }

    // helper to get initial model if none was provided
    function getDefaultModel(config) {
        if (config.startWithAddButton)
            return { fieldsets: [] };

        return { fieldsets: [getEmptyRenderFieldset(config.fieldsets[0])] };
    }

    //helper to add an empty fieldset to the render model
    function getEmptyRenderFieldset (fieldsetModel) {
        return {alias: fieldsetModel.alias, collapse: false, isValid: true, properties: []};
    }

    //helper to ensure no nulls make it into the model
    function removeNulls(model){
        if(model.fieldsets){
            _.each(model.fieldsets, function(fieldset, index){
                if(!fieldset){
                    model.fieldsets.splice(index, 1);
                    removeNulls(model);
                }
            });

            return model;
        }
    }

    // Hack for U4-4281 / #61
    function fixDisableSelection() {
        $timeout(function() {
            $('.archetypeEditor .controls')
                .bind('mousedown.ui-disableSelection selectstart.ui-disableSelection', function(e) {
                    e.stopImmediatePropagation();
                });
        }, 1000);
    }

    //helper to lookup validity when given a fieldsetIndex and property alias
    $scope.getPropertyValidity = function(fieldsetIndex, alias)
    {
        if($scope.model.value.fieldsets[fieldsetIndex])
        {
            var property = _.find($scope.model.value.fieldsets[fieldsetIndex].properties, function(property){
                return property.alias == alias;
            });
        }

        return (typeof property == 'undefined') ? true : property.isValid;
    }

    //helper to lookup validity when given a fieldset
    $scope.getFieldsetValidity = function (fieldset) {

        // Variables.
        var valid = true;

        // Recursive validation of nested fieldsets.
        recurseFieldsets(function(item) {
            if (item.isValid === false) {
                valid = false;
            }
        }, [fieldset]);

        // Were all the nested fieldsets valid?
        return valid;

    };

    // helper to force the current form into the dirty state
    $scope.setDirty = function () {
        if($scope.form) {
            $scope.form.$setDirty();
        }
    }

    $scope.minFieldsetsFulfilled = function () {
        if (!$scope.model.config.minFieldsets) {
            return true;
        }
        return $scope.numberOfEnabledFieldsets() >= $scope.model.config.minFieldsets;
    }

    //custom js
    if ($scope.model.config.customJsPath) {
        assetsService.loadJs($scope.model.config.customJsPath);
    }

    //archetype css
    assetsService.loadCss("../App_Plugins/Archetype/css/archetype.css");

    //custom css
    if($scope.model.config.customCssPath)
    {
        assetsService.loadCss($scope.model.config.customCssPath);
    }

    // submit watcher handling:
    // because some property editors use the "formSubmitting" event to set/clean up their model.value,
    // we need to monitor the "formSubmitting" event from a custom property and broadcast our own event
    // to forcefully update the appropriate model.value's
    $scope.activeSubmitWatcher = 0;
    $scope.submitWatcherOnLoad = function () {
        $scope.activeSubmitWatcher++;
        return $scope.activeSubmitWatcher;
    }
    $scope.submitWatcherOnSubmit = function (args) {
        $scope.$broadcast("archetypeFormSubmitting", args);
    }

    // we'll use our own "archetypeFormSubmitting" event to save custom properties, as at least some 
    // of the editors store their values back to the model on the core "formSubmitting" event
    $scope.$on("archetypeFormSubmitting", function (ev, args) {
        _.each($scope.model.value.fieldsets, function (fieldset) {
            // extract the publish configuration from the fieldsets (and convert local datetimes to UTC)
            fieldset.releaseDate = toUtc(fieldset.releaseDateModel.value);
            fieldset.expireDate = toUtc(fieldset.expireDateModel.value);
            // extract the allowed member groups 
            fieldset.allowedMemberGroups = fieldset.allowedMemberGroupsModel.value;
        });
    });

    // handle mandatory validation of the entire Archetype
    // - no fieldsets = not valid
    $scope.handleMandatoryValidation = function() {
        var valid = true;
        if ($scope.model.validation && $scope.model.validation.mandatory) {
            valid = $scope.numberOfEnabledFieldsets() > 0;
        }
        $scope.model.mandatoryValidation = valid ? "valid" : null;
    }

    function toUtc(date) {
        if (!date) {
            return null;
        }
        return moment(date, "YYYY-MM-DD HH:mm:ss").utc().toDate();
    }

    function fromUtc(date) {
        if (!date) {
            return null;
        }
        return moment(moment.utc(date).toDate()).format("YYYY-MM-DD HH:mm:ss")
    }

    // Serializes and deserializes an item to return a snapshot of that item (e.g., so it is not
    // changed before being inspected). Useful when troubleshooting.
    // Modified from: http://stackoverflow.com/a/11616993/2052963
    function jsonSnapshot(item) {
        var cache = [];
        var stringItem = JSON.stringify(item, function(key, value) {
            if (typeof value === "object" && value !== null) {
                if (cache.indexOf(value) !== -1) {
                    return "[Removed Circular Reference Item]";
                }
                cache.push(value);
            }
            return value;
        });
        return JSON.parse(stringItem);
    }

    // Recursively processes each Archetype fieldset.
    function recurseFieldsets(fn, fieldsets) {
        if (!fieldsets || !fieldsets.length) {
            return;
        }
        _.each(fieldsets, function(fieldset) {
            fn(fieldset);
            _.each(fieldset.properties, function (property) {
                if (property != null && property.value != null && property.propertyEditorAlias === "Imulus.Archetype") {
                    recurseFieldsets(fn, property.value.fieldsets);
                }
            });
        });
    }

    // Recursively processes each Archetype fieldset property.
    function recurseProperties(fn, fieldsets) {
        recurseFieldsets(function(fieldset) {
            _.each(fieldset.properties, function (property) {
                fn(property, fieldset);
            });
        }, fieldsets);
    }

    // Indicates whether or not the fieldset can be moved between the source and target scope.
    // The source scope is the scope of the fieldset being dragged.
    // The target scope is the scope of the Archetype to drop into.
    function canMove(sourceScope, targetScope) {
        var targetFieldsetConfigs = targetScope.model.config.fieldsets;
        var targetFieldsets = targetScope.model.value.fieldsets;
        var sourceFieldset = sourceScope.fieldset;
        var sameArchetype = arrayContainsFieldset(sourceFieldset, targetFieldsets);
        var model = sourceScope.fieldsetConfigModel;
        var canRemove = sourceScope.canRemove();
        var canAdd = targetScope.canAdd();
        var valid = sameArchetype || (canRemove && canAdd);
        valid = valid && modelMatchesAnyFieldset(model, targetFieldsetConfigs);
        return valid;
    }

    // This is an alternative to: element.sortable("instance")
    // Workaround for this issue: https://github.com/imulus/Archetype/pull/356#issuecomment-218527910
    // Snagged from this pull request: https://github.com/angular-ui/ui-sortable/pull/319
    // More technical details here: https://github.com/angular-ui/ui-sortable/issues/316
    function getSortableWidgetInstance(element) {
        // this is a fix to support jquery-ui prior to v1.11.x
        // otherwise we should be using `element.sortable('instance')`
        var data = element.data('ui-sortable');
        if (data && typeof data === 'object' && data.widgetFullName === 'ui-sortable') {
            return data;
        }
        return null;
    }
});

angular.module("umbraco").controller("Imulus.ArchetypeConfigController", function ($scope, $http, assetsService, dialogService, archetypePropertyEditorResource, archetypeGlobalConfigService, notificationsService) {

    //$scope.model.value = "";
    //console.log($scope.model.value);

    //define empty items
    var newPropertyModel = '{"alias": "", "remove": false, "collapse": false, "label": "", "helpText": "", "dataTypeGuid": "0cc0eba1-9960-42c9-bf9b-60e150b429ae", "value": ""}';
    var newFieldsetModel = '{"alias": "", "remove": false, "collapse": false, "labelTemplate": "", "icon": "", "label": "", "properties": [' + newPropertyModel + '], "group": null}';
    var defaultFieldsetConfigModel = JSON.parse('{"showAdvancedOptions": false, "startWithAddButton": false, "hideFieldsetToolbar": false, "enableMultipleFieldsets": false, "hideFieldsetControls": false, "hidePropertyLabel": false, "maxFieldsets": null, "enableCollapsing": true, "enableMultipleOpen": true, "enableCloning": false, "enableDisabling": true, "enableDeepDatatypeRequests": false, "enablePublishing": false, "enableMemberGroups": false, "enableCrossDragging": false, "fieldsets": [' + newFieldsetModel + '], "fieldsetGroups": []}');

    //ini the model
    $scope.model.value = $scope.model.value || defaultFieldsetConfigModel;

    $scope.dllVersion = "";

    archetypePropertyEditorResource.getDllVersion().then(function(data){
        $scope.dllVersion = data.dllVersion;
    });
       
    archetypeGlobalConfigService.checkForUpdates().then(function(data) {
        if(data.isUpdateAvailable) {
            notificationsService.add({
                headline: data.headline,
                type: data.type,
                message: data.message,
                url: data.url
            });
        }
    });

    //ini the render model
    initConfigRenderModel();

    //get the available datatypes
    archetypePropertyEditorResource.getAllDataTypes().then(function(data) {
        $scope.availableDataTypes = data;
    });

    //iconPicker
    $scope.selectIcon = function(fieldset){
        var dialog = dialogService.iconPicker({
            callback: function(data){
                fieldset.icon = data;
            }
        });
    }

    //config for the sorting
    $scope.sortableOptions = {
        axis: 'y',
        cursor: "move",
        handle: ".handle",
        update: function (ev, ui) {

        },
        stop: function (ev, ui) {

        }
    };

    // added to track loaded fieldsets 
    $scope.loadedFieldsets = [];
    $scope.isLoaded = function (fieldset) {
        return $scope.loadedFieldsets.indexOf(fieldset) >= 0;
    }

    //function that determines how to manage expanding/collapsing fieldsets
    $scope.focusFieldset = function(fieldset){
        var iniState;

        if(fieldset)
        {
            iniState = fieldset.collapse;
        }

        _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset){
            if($scope.archetypeConfigRenderModel.fieldsets.length == 1 && fieldset.remove == false)
            {
                fieldset.collapse = false;
                $scope.loadedFieldsets.push(fieldset);
                return;
            }

            if(fieldset.label)
            {
                fieldset.collapse = true;
            }
            else
            {
                fieldset.collapse = false;
                $scope.loadedFieldsets.push(fieldset);
            }
        });

        if(iniState)
        {
            fieldset.collapse = !iniState;
            $scope.loadedFieldsets.push(fieldset);
        }
    }

    //ini the fieldsets
    $scope.focusFieldset();

    //function that determines how to manage expanding/collapsing properties
    $scope.focusProperty = function(properties, property){
        var iniState;

        if(property)
        {
            iniState = property.collapse;
        }

        _.each(properties, function(property){
            if(property.label)
            {
                property.collapse = true;
            }
            else
            {
                property.collapse = false;
            }
        });

        if(iniState)
        {
            property.collapse = !iniState;
        }
    }

    //ini the properties
    _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset){
        $scope.focusProperty(fieldset.properties);
    });

    //setup JSON.stringify helpers
    $scope.archetypeConfigRenderModel.toString = stringify;

    //encapsulate stringify (should be built into browsers, not sure of IE support)
    function stringify() {
        return JSON.stringify(this);
    }

    //watch for changes
    $scope.$watch('archetypeConfigRenderModel', function (v) {
        if (typeof v === 'string') {
            $scope.archetypeConfigRenderModel = JSON.parse(v);
            $scope.archetypeConfigRenderModel.toString = stringify;
        }
    });

    $scope.autoPopulateAlias = function (modelProperty) {
        if (modelProperty && !modelProperty.aliasIsDirty) {
            modelProperty.alias = modelProperty.label.toUmbracoAlias();
        }
    }

    $scope.markAliasDirty = function (modelProperty) {
        if (modelProperty && !modelProperty.aliasIsDirty) {
            modelProperty.aliasIsDirty = true;;
        }
    }

    //helper that returns if an item can be removed
    $scope.canRemoveFieldset = function ()
    {
        return countVisibleFieldset() > 1;
    }

    //helper that returns if an item can be sorted
    $scope.canSortFieldset = function ()
    {
        return countVisibleFieldset() > 1;
    }

    //helper that returns if an item can be removed
    $scope.canRemoveProperty = function (fieldset)
    {
        return countVisibleProperty(fieldset) > 1;
    }

    //helper that returns if an item can be sorted
    $scope.canSortProperty = function (fieldset)
    {
        return countVisibleProperty(fieldset) > 1;
    }

    $scope.getDataTypeNameByGuid = function (guid) {
        if ($scope.availableDataTypes == null) // Might not be initialized yet?
            return "";
        
        var dataType = _.find($scope.availableDataTypes, function(d) {
            return d.guid == guid;
        });

        return dataType == null ? "" : dataType.name;
    }

    //helper to count what is visible
    function countVisibleFieldset()
    {
        var count = 0;

        _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset){
            if (fieldset.remove == false) {
                count++;
            }
        });

        return count;
    }

    //determines how many properties are visible
    function countVisibleProperty(fieldset)
    {
        var count = 0;

        for (var i in fieldset.properties) {
            if (fieldset.properties[i].remove == false) {
                count++;
            }
        }

        return count;
    }

    //handles a fieldset add
    $scope.addFieldsetRow = function ($index, $event) {
        $scope.archetypeConfigRenderModel.fieldsets.splice($index + 1, 0, JSON.parse(newFieldsetModel));
        $scope.focusFieldset();
    }

    //rather than splice the archetypeConfigRenderModel, we're hiding this and cleaning onFormSubmitting
    $scope.removeFieldsetRow = function ($index) {
        if ($scope.canRemoveFieldset()) {
            if (confirm('Are you sure you want to remove this?')) {
                $scope.archetypeConfigRenderModel.fieldsets[$index].remove = true;
            }
        }
    }

    //handles a property add
    $scope.addPropertyRow = function (fieldset, $index) {
        fieldset.properties.splice($index + 1, 0, JSON.parse(newPropertyModel));
    }

    //rather than splice the archetypeConfigRenderModel, we're hiding this and cleaning onFormSubmitting
    $scope.removePropertyRow = function (fieldset, $index) {
        if ($scope.canRemoveProperty(fieldset)) {
            if (confirm('Are you sure you want to remove this?')) {
                fieldset.properties[$index].remove = true;
            }
        }
    }

    //helper to ini the render model
    function initConfigRenderModel()
    {
        $scope.archetypeConfigRenderModel = $scope.model.value;
        if (!$scope.archetypeConfigRenderModel.fieldsetGroups) {
            $scope.archetypeConfigRenderModel.fieldsetGroups = [];
        }

        _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset) {

            if (fieldset.group) {
                // tie the fieldset group back up to the actual group object, not the clone that's been persisted
                fieldset.group = _.find($scope.archetypeConfigRenderModel.fieldsetGroups, function(fieldsetGroup) {
                    return fieldsetGroup.name == fieldset.group.name;
                })
            }

            fieldset.remove = false;
            if (fieldset.alias.length > 0)
                fieldset.aliasIsDirty = true;

            if(fieldset.label)
            {
                fieldset.collapse = true;
            }

            _.each(fieldset.properties, function(fieldset){
                fieldset.remove = false;
                if (fieldset.alias.length > 0)
                    fieldset.aliasIsDirty = true;
            });
        });
    }

    //sync things up on save
    $scope.$on("formSubmitting", function (ev, args) {
        syncModelToRenderModel();
    });

    //helper to sync the model to the renderModel
    function syncModelToRenderModel()
    {
        $scope.model.value = $scope.archetypeConfigRenderModel;
        var fieldsets = [];

        _.each($scope.archetypeConfigRenderModel.fieldsets, function(fieldset){
            //check fieldsets
            if (!fieldset.remove) {
                fieldsets.push(fieldset);

                var properties = [];

                _.each(fieldset.properties, function(property){
                   if (!property.remove) {
                        properties.push(property);
                    }
                });

                fieldset.properties = properties;
            }
        });

        $scope.model.value.fieldsets = fieldsets;
    }

    $scope.showOptions = function ($event, template) {
        $event.preventDefault();

        dialogService.closeAll();

        dialogService.open({
            template: template,
            show: true,
            callback: function (data) {
                // replace the entire render model if it was changed (in developer options)
                if (data.model && data.modelChanged) {
                    $scope.archetypeConfigRenderModel = data.model;
                }
            },
            dialogData: { model: $scope.archetypeConfigRenderModel }
        });
    }

    //archetype css
    assetsService.loadCss("../App_Plugins/Archetype/css/archetype.css");
});

angular.module('umbraco').controller('ArchetypeConfigOptionsController', function ($scope) {
    
    //handles a fieldset group add
    $scope.addFieldsetGroup = function () {
        $scope.dialogData.model.fieldsetGroups.push({ name: "" });
    }

    //handles a fieldset group removal
    $scope.removeFieldsetGroup = function ($index) {
        $scope.dialogData.model.fieldsetGroups.splice($index, 1);
    }

    $scope.apply = function(index) {
        $scope.submit($scope.dialogData);
    }
});
angular.module('umbraco').controller('ArchetypeConfigGlobalOptionsController', function ($scope, archetypeGlobalConfigService) {
    $scope.globalSettings = {};
  
    $scope.confirmCheckNewVersionChange = function() {      
        archetypeGlobalConfigService.setCheckForUpdates($scope.globalSettings.checkForNewVersion);
    }
    
    function getGlobalSettings()
    {
        archetypeGlobalConfigService.globalSettings().then(function(data) {          
            $scope.globalSettings.checkForNewVersion = data.isCheckingForUpdates;
        });
    }    
    
    init();
       
    function init()
    {      
        getGlobalSettings();
    }
});
angular.module("umbraco.directives").directive('archetypeProperty', function ($compile, $http, archetypePropertyEditorResource, umbPropEditorHelper, $timeout, $rootScope, $q, fileManager, editorState, archetypeService) {

    var linker = function (scope, element, attrs, ngModelCtrl) {
        var configFieldsetModel = archetypeService.getFieldsetByAlias(scope.archetypeConfig.fieldsets, scope.fieldset.alias);
        var view = "";
        var label = configFieldsetModel.properties[scope.propertyConfigIndex].label;
        var dataTypeGuid = configFieldsetModel.properties[scope.propertyConfigIndex].dataTypeGuid;
        var config = null;
        var alias = configFieldsetModel.properties[scope.propertyConfigIndex].alias;
        var defaultValue = configFieldsetModel.properties[scope.propertyConfigIndex].value;
        var propertyAliasParts = [];
        var propertyAlias = archetypeService.getUniquePropertyAlias(scope, propertyAliasParts);
        
        //try to convert the defaultValue to a JS object
        defaultValue = archetypeService.jsonOrString(defaultValue, scope.archetypeConfig.developerMode, "defaultValue");

        //grab info for the selected datatype, prepare for view
        archetypePropertyEditorResource.getDataType(dataTypeGuid, scope.archetypeConfig.enableDeepDatatypeRequests, editorState.current.contentTypeAlias, scope.propertyEditorAlias, alias, editorState.current.id).then(function (data) {
            //transform preValues array into object expected by propertyeditor views
            var configObj = {};

            _.each(data.preValues, function(p) {
                configObj[p.key] = p.value;
            });
            
            config = configObj;

            //determine the view to use [...] and load it
            archetypePropertyEditorResource.getPropertyEditorMapping(data.selectedEditor).then(function(propertyEditor) {
                var pathToView = umbPropEditorHelper.getViewPath(propertyEditor.view);

                //load in the DefaultPreValues for the PropertyEditor, if any
                var defaultConfigObj =  {};
                if (propertyEditor.hasOwnProperty('defaultPreValues') && propertyEditor.defaultPreValues != null) {
                    _.extend(defaultConfigObj, propertyEditor.defaultPreValues);
                }

                var mergedConfig = _.extend(defaultConfigObj, config);

                loadView(pathToView, mergedConfig, defaultValue, alias, propertyAlias, scope, element, ngModelCtrl, configFieldsetModel);
            });
        });
    }

    function loadView(view, config, defaultValue, alias, propertyAlias, scope, element, ngModelCtrl, configFieldsetModel) {
        if (view)
        {
            $http.get(view, { cache: true }).success(function (data) {
                if (data) {
                    if (scope.archetypeConfig.developerMode == '1')
                    {
                        console.log(scope);
                    }

                    //define the initial model and config
                    scope.form = scope.umbracoForm;
                    scope.model = {};
                    scope.model.config = {};

                    //ini the property value after test to make sure a prop exists in the renderModel
                    scope.renderModelPropertyIndex = archetypeService.getPropertyIndexByAlias(archetypeService.getFieldset(scope).properties, alias);

                    if (!scope.renderModelPropertyIndex)
                    {
                        var propertyValue = { alias: alias, value: defaultValue };
                        archetypeService.getFieldset(scope).properties.push(propertyValue);
                        scope.renderModelPropertyIndex = archetypeService.getPropertyIndexByAlias(archetypeService.getFieldset(scope).properties, alias);
                    }

                    scope.renderModel = {};
                    scope.model.value = archetypeService.getFieldsetProperty(scope).value;

                    // init the property editor state (while ensuring the temporary ID is retained
                    // from any prior initializations).
                    var fieldsetProperty = archetypeService.getFieldsetProperty(scope);
                    var oldState = fieldsetProperty.editorState;
                    archetypeService.ensureTemporaryId(fieldsetProperty);
                    if (oldState) {
                        fieldsetProperty.editorState.temporaryId = oldState.temporaryId;
                    }

                    //set the config from the prevalues
                    scope.model.config = config;

                    /*
                        Property Specific Hacks

                        Preference is to not do these, but unless we figure out core issues, this is quickest fix.
                    */

                    //MNTP *single* hack
                    if(scope.model.config.maxNumber && scope.model.config.multiPicker)
                    {
                        if(scope.model.config.maxNumber == "1")
                        {
                            scope.model.config.multiPicker = "0";
                        }
                    }

                    //hacks for various built-in datatyps including upload, colorpicker and tags
                    if (!scope.propertyForm) {
                        scope.propertyForm = scope.form;
                    }
                    if (!scope.model.validation) {
                        scope.model.validation = {};
                        scope.model.validation.mandatory = 0;
                    }

                    //some items need an alias
                    scope.model.alias = "archetype-property-" + propertyAlias;
                    //some items also need an id (file upload for example)
                    scope.model.id = propertyAlias;

                    //watch for changes since there is no two-way binding with the local model.value
                    scope.$watch('model.value', function (newValue, oldValue) {
                        
                        archetypeService.getFieldsetProperty(scope).value = newValue;

                        // notify the linker that the property value changed
                        archetypeService.propertyValueChanged(archetypeService.getFieldset(scope), archetypeService.getFieldsetProperty(scope));
                    });

                    scope.$on('archetypeFormSubmitting', function (ev, args) {
                        // #385 - revert the changes made in #311 to avoid publishing invalid fieldsets (leaving the code from #311 here, in case we figure out how to re-introduce "save as draft")
                        // if(args.action !== 'save') {
                        // validate all fieldset properties
                        _.each(scope.fieldset.properties, function (property) {
                            archetypeService.validateProperty(scope.fieldset, property, configFieldsetModel);
                        });

                        var validationKey = "validation-f" + scope.fieldsetIndex;

                        ngModelCtrl.$setValidity(validationKey, scope.fieldset.isValid);
                        // }

                        // did the value change (if it did, it most likely did so during the "formSubmitting" event)
                        var property = archetypeService.getFieldsetProperty(scope);

                        var currentValue = property.value;

                        if (currentValue != scope.model.value) {
                            archetypeService.getFieldsetProperty(scope).value = scope.model.value;

                            // notify the linker that the property value changed
                            archetypeService.propertyValueChanged(archetypeService.getFieldset(scope), archetypeService.getFieldsetProperty(scope));
                        }
                    });

                    // issue 114: handle file selection on property editors
                    scope.$on("filesSelected", function (event, args) {
                        // populate the fileNames collection on the property editor state
                        var property = archetypeService.getFieldsetProperty(scope);

                        property.editorState.fileNames = [];

                        _.each(args.files, function (item) {
                            property.editorState.fileNames.push(item.name);
                        });

                        // remove the files set for this property
                        // NOTE: we can't use property.alias because the file manager registers the selected files on the assigned Archetype property alias (e.g. "archetype-property-archetype-property-archetype-property-content-0-2-0-1-0-0")
                        fileManager.setFiles(scope.model.alias, []);

                        // now tell the containing Archetype to pick up the selected files
                        scope.archetypeRenderModel.setFiles(args.files);
                    });

                    scope.$on("archetypeRemoveFieldset", function (ev, args) {
                        var validationKey = "validation-f" + args.index;
                        ngModelCtrl.$setValidity(validationKey, true);
                    });

                    element.html(data).show();
                    $compile(element.contents())(scope);

                    $timeout(function() {
                        var def = $q.defer();
                        def.resolve(true);
                        $rootScope.$apply();
                    }, 500);
                }
            });
        }
    }

    return {
        require: "^ngModel",
        restrict: "E",
        replace: true,
        link: linker,
        scope: {
            property: '=',
            propertyConfigIndex: '=',
            propertyEditorAlias: '=',
            archetypeConfig: '=',
            fieldset: '=',
            fieldsetIndex: '=',
            archetypeRenderModel: '=',
            umbracoPropertyAlias: '=',
            umbracoForm: '='
        }
    }
});
angular.module("umbraco.directives").directive('archetypeSubmitWatcher', function ($rootScope) {
    var linker = function (scope, element, attrs, ngModelCtrl) {
        // call the load callback on scope to obtain the ID of this submit watcher
        var id = scope.loadCallback ? scope.loadCallback() : 0;

        scope.$on("formSubmitting", function (ev, args) {
            // on the "formSubmitting" event, call the submit callback on scope to notify the Archetype controller to do it's magic
            if (id == scope.activeSubmitWatcher) {
                scope.submitCallback(args);
            }
        });
    }

    return {
        restrict: "E",
        replace: true,
        link: linker,
        template: "",
        scope: {
            loadCallback: '=',
            submitCallback: '=',
            activeSubmitWatcher: '='
        }
    }
});
angular.module("umbraco.directives").directive('archetypeCustomView', function ($compile, $http) {
    var linker = function (scope, element, attrs) {

        var view = "../App_plugins/Archetype/views/archetype.default.html";
        if(scope.model.config.customViewPath) {
            view = scope.model.config.customViewPath;
        }

        $http.get(view, { cache: true }).then(function(data) {

            element.html(data.data).show();

            $compile(element.contents())(scope);
        });
    }

    return {
        restrict: "A",
        replace: true,
        link: linker
    }
});
angular.module("umbraco.directives").directive('archetypeLocalize', function (archetypeLocalizationService) {
	var linker = function (scope, element, attrs){

		var key = scope.key;
        
        archetypeLocalizationService.localize(key).then(function(value){
        	if(value){
        		element.html(value);
        	}
        });
	}   

	return {
	    restrict: "E",
	    replace: true,
	    link: linker,
	    scope: {
	    	key: '@'
	    }
	}
});
angular.module("umbraco.directives")
    .directive('archetypeClickOutside', function ($timeout, $parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs, ctrl) {
                var fn = $parse(attrs.archetypeClickOutside);

                // add click event handler (delayed so we don't trigger the callback immediately if this directive itself was triggered by a mouse click)
                $timeout(function () {
                  $(document).on("click", mouseClick);
                }, 500);

                function mouseClick(event) {  
                    if($(event.target).closest(element).length > 0) {
                        return;
                    }
                    var callback = function () {
                        fn(scope, { $event: event });
                    };
                    scope.$apply(callback);
                }

                // unbind event
                scope.$on('$destroy', function () {
                    $(document).off("click", mouseClick);
                });
            }
        };
    });
angular.module('umbraco.services').factory('archetypeLocalizationService', function($http, $q, userService){
    var service = {
        resourceFileLoaded: false,
        dictionary: {},
        localize: function(key) {
            var deferred = $q.defer();

            if(service.resourceFileLoaded){
                var value = service._lookup(key);
                deferred.resolve(value);
            }
            else{
                service.initLocalizedResources().then(function(dictionary){
                   var value = service._lookup(key);
                   deferred.resolve(value); 
                });
            } 

            return deferred.promise;
        },
        _lookup: function(key){
            return service.dictionary[key];
        },
        initLocalizedResources:function () {
            var deferred = $q.defer();
            userService.getCurrentUser().then(function(user){
                $http.get("../App_plugins/Archetype/langs/" + user.locale + ".js", { cache: true }) 
                    .then(function(response){
                        service.resourceFileLoaded = true;
                        service.dictionary = response.data;

                        return deferred.resolve(service.dictionary);
                    }, function(err){
                        return deferred.reject("Lang file missing");
                    });
            });
            return deferred.promise;
        }
    }

    return service;
});
//create a namespace (optional)
var ArchetypeSampleLabelHelpers = {};

//create a function
//you will add it to your label template field as `{{ArchetypeSampleLabelHelpers.testPromise(someArchetypePropertyAlias)}}`
ArchetypeSampleLabelHelpers.testPromise = function(value) {   
    //you can inject services
    return function ($timeout, archetypeCacheService) {        
        //best to return a promise
        //NOTE: $timeout returns a promise
        return $timeout(function () {
            return "As Promised: " + value;
        }, 1000);
    }
}

ArchetypeSampleLabelHelpers.testEntityPromise = function(value, scope, args) {
    //hey look, args!
    //{{ArchetypeSampleLabelHelpers.testEntityPromise(someArchetypePropertyAlias, {foo: 1})}}
    console.log(args);
    
    return function ($q, entityResource) {    
        var deferred = $q.defer();
    
        entityResource.getById(args.foo, 'document').then(function(entity) {
            console.log("Hello from testEntityPromise");
            console.log(entity);
            deferred.resolve(entity.name);
        });
    
        return deferred.promise;
    }
}

ArchetypeSampleLabelHelpers.testEntityPromise2 = function(value, scope, args) {  
    //hey look, args but we're also using the built-in archetypeCacheService
    //{{ArchetypeSampleLabelHelpers.testEntityPromise(someArchetypePropertyAlias, {foo: 1234})}}
    console.log(args);        
    
    return function ($q, archetypeCacheService) {    
        var deferred = $q.defer();
    
        archetypeCacheService.getEntityById(args.foo, 'document').then(function(entity) {
            console.log("Hello from testEntityPromise2");
            console.log(entity);
            deferred.resolve(entity.name);
        });
    
        return deferred.promise;
    }
}
angular.module('umbraco.resources').factory('archetypePropertyEditorResource', function($q, $http, umbRequestHelper){
    return { 
        getAllDataTypes: function() {
            // Hack - grab DataTypes from Tree API, as `dataTypeService.getAll()` isn't implemented yet
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetAll"), 'Failed to retrieve datatypes from tree service'
            );
        },
        getAllDataTypesForCache: function() {
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetAllDetails"), 'Failed to retrieve datatypes from tree service'
            );
        },
        getDataType: function (guid, useDeepDatatypeLookup, contentTypeAlias, propertyTypeAlias, archetypeAlias, nodeId) {
            if(useDeepDatatypeLookup) {
            	return umbRequestHelper.resourcePromise(
            		$http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetByGuid?guid=" + guid + "&contentTypeAlias=" + contentTypeAlias + "&propertyTypeAlias=" + propertyTypeAlias + "&archetypeAlias=" + archetypeAlias + "&nodeId=" + nodeId), 'Failed to retrieve datatype'
        		);
            }
            else {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetByGuid?guid=" + guid , { cache: true }), 'Failed to retrieve datatype'
                );
            }
        },
        getPropertyEditorMapping: function(alias) {
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetAllPropertyEditors", { cache: true }), 'Failed to retrieve datatype mappings'
            ).then(function (data) {
                var result = _.find(data, function(d) {
                    return d.alias === alias;
                });

                if (result != null) 
                    return result;

                return "";
            });
        },
        getDllVersion: function() {
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/GetDllVersion", { cache: true }), 'Failed to retrieve dll version'
            );
        }
    }
}); 

angular.module('umbraco.services').factory('archetypeService', function () {

    // Variables.
    var draggedRteArchetype;
    var archetypeEditors;
    var rteClass = ".archetypeEditor .umb-rte textarea";
    var editorSettings = {};
    var disabledSortables = [];

    //public
    return {
        //helper that returns a JS ojbect from 'value' string or the original string
        jsonOrString: function (value, developerMode, debugLabel){
            if(value && typeof value == 'string'){
                try{
                    if(developerMode == '1'){
                        console.log("Trying to parse " + debugLabel + ": " + value); 
                    }
                    value = JSON.parse(value);
                }
                catch(exception)
                {
                    if(developerMode == '1'){
                        console.log("Failed to parse " + debugLabel + "."); 
                    }
                }
            }

            if(value && developerMode == '1'){
                console.log(debugLabel + " post-parsing: ");
                console.log(value); 
            }

            return value;
        },
        getFieldsetByAlias: function (fieldsets, alias)
        {
            return _.find(fieldsets, function(fieldset){
                return fieldset.alias == alias;
            });
        },
        getPropertyIndexByAlias: function(properties, alias){
            for (var i in properties)
            {
                if (properties[i].alias == alias) {
                    return i;
                }
            }
        },
        getPropertyByAlias: function (fieldset, alias){
            return _.find(fieldset.properties, function(property){
                return property.alias == alias; 
            });
        },
        getUniquePropertyAlias: function (currentScope, propertyAliasParts, excludeUniqueId) {
            if (currentScope.hasOwnProperty('fieldsetIndex') && currentScope.hasOwnProperty('property') && currentScope.hasOwnProperty('propertyConfigIndex'))
            {
                var currentPropertyAlias = "f" + currentScope.fieldsetIndex + "-" + currentScope.property.alias + "-p" + currentScope.propertyConfigIndex;
                propertyAliasParts.push(currentPropertyAlias);
            }
            else if (currentScope.hasOwnProperty('isPreValue')) // Crappy way to identify this is the umbraco property scope
            {
                var umbracoPropertyAlias = currentScope.$parent.$parent.property.alias; // Crappy way to get the umbraco host alias once we identify its scope
                propertyAliasParts.push(umbracoPropertyAlias);
            }

            if (currentScope.$parent)
                this.getUniquePropertyAlias(currentScope.$parent, propertyAliasParts, true);

            var reversed = _.unique(propertyAliasParts).reverse();

            if (!excludeUniqueId) {
                reversed.push(_.uniqueId("u-"));
            }

            return reversed.join("-");
        },
        getFieldset: function(scope) {
            var renderModel = scope.archetypeRenderModel;
            return renderModel ? renderModel.fieldsets[scope.fieldsetIndex] : null;
        },
        getFieldsetProperty: function (scope) {
            var fieldset = this.getFieldset(scope);
            return fieldset ? fieldset.properties[scope.renderModelPropertyIndex] : null;
        },
        setFieldsetValidity: function (fieldset) {
            // mark the entire fieldset as invalid if there are any invalid properties in the fieldset, otherwise mark it as valid
            fieldset.isValid =
                _.find(fieldset.properties, function (property) {
                    return property.isValid == false
                }) == null;
        },
        validateProperty: function (fieldset, property, configFieldsetModel) {
            var propertyConfig = this.getPropertyByAlias(configFieldsetModel, property.alias);

            if (propertyConfig) {
                // use property.value !== property.value to check for NaN values on numeric inputs
                if (propertyConfig.required && (property.value == null || property.value === "" || property.value !== property.value)) {
                    property.isValid = false;
                }
                // issue 116: RegEx validate property value
                // Only validate the property value if anything has been entered - RegEx is considered a supplement to "required".
                if (property.isValid == true && propertyConfig.regEx && property.value) {
                    var regEx = new RegExp(propertyConfig.regEx);
                    if (regEx.test(property.value) == false) {
                        property.isValid = false;
                    }
                }
            }

            this.setFieldsetValidity(fieldset);
        },
        // called when the value of any property in a fieldset changes
        propertyValueChanged: function (fieldset, property) {
            // it's the Umbraco way to hide the invalid state when altering an invalid property, even if the new value isn't valid either
            property.isValid = true;
            this.setFieldsetValidity(fieldset);
        },
        // This stores the rich text editors in all Archetypes (e.g., during a drag operation).
        // Typically, the editors will be restored after the drag completes.
        storeEditors: function (element) {

            // If there are not tinyMCE editors, this may be undefined.
            if (typeof tinyMCE === "undefined") {
                return;
            }

            // Variables.
            var self = this;
            draggedRteArchetype = element;

            // Empty the stored settings.
            editorSettings = {};

            // For fast lookups, store each editor by the element ID.
            var editorsById = {};
            for (var i = 0; i < tinyMCE.editors.length; i++) {
                var tempEditor = tinyMCE.editors[i];
                editorsById[tempEditor.id] = tempEditor;
            }

            // Process each rich text editor.
            $(rteClass).each(function() {

                // Variables.
                var id = $(this).attr("id");
                var editor = editorsById[id];

                // Get the property's temporary ID.
                var scope = angular.element(this).scope().$parent;
                var property = self.getFieldsetProperty(scope);
                var tempId = property ? property.editorState.temporaryId : null;

                // Store the editor settings by the temporary ID?
                if (editor && editor.settings && tempId) {
                    editorSettings[tempId] = editor.settings;
                }

            });

        },
        // This restores the rich text editors in the specified Archetype
        // (e.g., after a drop drop operation).
        restoreEditors: function(element) {

            // If there are not tinyMCE editors, this may be undefined.
            if (typeof tinyMCE === "undefined") {
                return;
            }

            // Variables.
            var bothElements = element
                ? element.add(draggedRteArchetype)
                : draggedRteArchetype;
            var self = this;

            // Process each RTE in both Archetypes.
            $(rteClass, bothElements).each(function () {

                // Variables.
                var id = $(this).attr("id");

                // Get the stored editor settings.
                var scope = angular.element(this).scope().$parent;
                var property = self.getFieldsetProperty(scope);
                var tempId = property ? property.editorState.temporaryId : null;
                var settings = editorSettings[tempId];

                // Remove and reinitialize the editor.
                if (settings) {
                    tinyMCE.execCommand("mceRemoveEditor", false, id);
                    tinyMCE.init(settings);
                }

            });

        },
        // Ensures the specified property has a temporary ID in its editor state,
        // optionally forcing one to be regenerated (if specified).
        ensureTemporaryId: function(property, regenerateId) {
            if (!property.editorState) {
                property.editorState = {};
            }
            var editorState = property.editorState;
            if (!editorState.temporaryId || regenerateId) {
                editorState.temporaryId = _.uniqueId("property-temp-id-");
            }
        },
        // Remembers a sortable that has been disabled (so it can be enabled later).
        rememberDisabledSortable: function(sortable) {
            disabledSortables.push(sortable);
        },
        // Enables all of the sortables that were disabled.
        enableSortables: function() {
            _.each(disabledSortables, function(sortable) {
                sortable.enable();
            });
            disabledSortables = [];
        },
        // Get all active archetype editors on the current page
        getEditors: function() {
            if (archetypeEditors == null) {
                archetypeEditors = $('.archetypeEditor');
            }
            return archetypeEditors;
        }
    }
});
angular.module('umbraco.services').factory('archetypeLabelService', function (archetypeCacheService, $q, $injector, $timeout) {
    //private

    /**
     * This will repeatedly wait for all promises in an array of promises to resolve, and it allows
     * for promises to be added to the array (i.e., if you add more promises, those added promises
     * will also be resolved too).
     * @param promises The array of promises.
     * @returns {*} The promise that will resolve once all promises in the array resolve.
     */
    function repeatedlyWaitForPromises(promises) {

        // Remember the original number of promises being resolved.
        var originalLength = promises.length;
        
        return $q.all(promises).then(function () {

            // If there are new promises, resolve those too.
            if (promises.length > originalLength) {
                promises = promises.slice(originalLength);
                return repeatedlyWaitForPromises(promises);
            }
        });
    }

    /**
     * Processes a value to be used in a fieldset label. Since it might be a promise or a function, the
     * value is repeatedly processed until it becomes a string.
     * @param labelValue The value to be used in the fieldset label.
     * @param promises The collection of promises to add any promises to.
     * @param match The current matched substring object in a label template.
     */
    function processLabelValue(labelValue, promises, match) {

        // Normalize null/undefined values to an empty string.
        if(!labelValue) {
            labelValue = "";
        }

        // Check the type of value (may be a string, promise, function, or other).
        if (isString(labelValue)) {

            // handle collapsing dollar signs in labels (#387)
            if (labelValue.indexOf("$$") >= 0) {
                labelValue = labelValue.replace(/\$\$/g, "$$$$$$$$");
            }

            // Set a new value now that it has been processed.
            //console.log("Processing string..." + labelValue);
            match.value = labelValue;
        } else if (isPromise(labelValue)) {
            // Remember the promise so we can wait for it to be completed before constructing the
            // fieldset label.
            promises.push(labelValue);
            labelValue.then(function (value) {
                //console.log("Processing final value..." + value);
                // The value will probably be a string, but recursively process it in case it's
                // something else.
                processLabelValue(value, promises, match);
            });
        } else if (_.isFunction(labelValue)) {

            // Allow for the function to accept injected parameters, and invoke it.
            labelValue = $injector.invoke(labelValue);

            // Recursively check result (may be a string, promise, or another function (another
            // function would be pretty strange, though I see no reason to disallow it).
            processLabelValue(labelValue, promises, match);
        } else {
            // Some other data type (e.g., number, date, object).
            match.value = labelValue;
        }
    }

    /**
     * Checks if the specified value is of type string.
     * @param value The value to check the type of.
     * @returns {boolean} True, if the value is a string; otherwise, false.
     */
    function isString(value) {
        if (value === null) {
            return false;
        } else if (typeof value === 'string') {
            return true;
        } else if (value instanceof String) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * Checks if the specified value is a JavaScript promise.
     * @param value The value that may be a promise.
     * @returns {*} True, if the value appears to be a promise; otherwise, false.
     */
    function isPromise(value) {
        return value && _.isFunction(value.then);
    }

    /**
     * Splits a string value into a collection based on a regular expression.
     * @param rgx The regular expression to use to find matches in the string value.
     * @param value The string value to split.
     * @returns {Array} A collection of objects, each representing a portion of the supplied
     *      string. Each object will contain the substring value, as well as a property
     *      indicating whether or not that substring was matched the regular expression.
     */
    function splitByRegex(rgx, value) {
        // Validate input.
        if (!rgx || !value) {
            return [];
        }

        // Variables.
        var substring,
            splitParts = [],
            nextIndex = 0,
            index;

        // Reset regex so we get all the matches.
        rgx.lastIndex = 0;

        // Loop through each match until there are no more matches.
        var match = rgx.exec(value);
        while (match) {

            // Extract match index.
            index = match.index;

            // Is there text between the prior match and this one?
            if (nextIndex < index) {
                substring = value.substring(nextIndex, index);
                splitParts.push({
                    isMatch: false,
                    value: substring
                });
            }

            // Remember the end of this match for the next loop iteration.
            nextIndex = rgx.lastIndex;

            // Store info about this match.
            substring = value.substring(index, nextIndex);
            splitParts.push({
                isMatch: true,
                value: substring
            });

            // Get next match.
            match = rgx.exec(value);
        }

        // The text after the last match.
        if (nextIndex < value.length) {
            substring = value.substring(nextIndex);
            splitParts.push({
                isMatch: false,
                value: substring
            });
        }

        // Reset regex in case somebody else wants to use it.
        rgx.lastIndex = 0;

        // Return information about the matches.
        return splitParts;
    }

    function executeFunctionByName(functionName, context) {
        var args = Array.prototype.slice.call(arguments).splice(2);

        var namespaces = functionName.split(".");
        var func = namespaces.pop();

        for(var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }

        if(context && context[func]) {
            return context[func].apply(this, args);
        }

        return "";
    }

    function getNativeLabel(datatype, value, scope) {
        switch (datatype.selectedEditor) {
                case "Imulus.UrlPicker":
                    return imulusUrlPicker(value, scope, {});
                case "Umbraco.TinyMCEv3":
                    return coreTinyMce(value, scope, {});
                case "Umbraco.MultiNodeTreePicker":
                    return coreMntp(value, scope, datatype);
                case "Umbraco.MultiNodeTreePicker2":
                    return coreMntpV2(value, scope, datatype);
                case "Umbraco.MultipleMediaPicker":
                case "Umbraco.MediaPicker":
                    return coreMediaPicker(value, scope, datatype);
                case "Umbraco.MediaPicker2":
                    return coreMediaPickerV2(value, scope, datatype);
                case "Umbraco.DropDown":
                    return coreDropdown(value, scope, datatype);
                case "RJP.MultiUrlPicker":
                    return rjpMultiUrlPicker(value, scope, {});
                case "Umbraco.ContentPickerAlias":
                    return coreContentPicker(value, scope, datatype);
                case "Umbraco.ContentPicker2":
                    return coreContentPickerV2(value, scope, datatype);
                default:
                    return null;
        }
    }

    function coreDropdown(value, scope, args) {

        if(!value)
            return "";

        var prevalue = args.preValues[0].value[value];

        if(prevalue) {
            return prevalue.value;
        }

        return "";
    }

    function coreMntp(value, scope, args) {
        var ids = value.split(',');
        var type = "Document";
        var deferred = $q.defer();

        switch(args.preValues[0].value.type) {
            case 'content':
                type = 'Document';
                break;
            case 'media':
                type = 'media';
                break;
            case 'member':
                type = 'member';
                break;
            default:
                break;
        }

        var entityArray = [];
        var promises = [];

        _.each(ids, function(id){
            if(id) {
               promises.push(archetypeCacheService.getEntityById(id, type).then(function(entity){                
                    if(entity) {
                        entityArray.push(entity.name);
                    }
               }).promise);
            }
        });

        $q.all(promises).then(function() {
            deferred.resolve(entityArray.join(', '));
        });
                
        return deferred.promise;
    }

    function coreMntpV2(value, scope, args) {
        var deferred = $q.defer();
        
        var ids = value.split(',');
        
        if (ids.length == 0) {
          return "";
        }
        
        var type = "document";

        switch(args.preValues[0].value.type) {
            case 'content':
                type = 'document';
                break;
            case 'media':
                type = 'media';
                break;
            case 'member':
                type = 'member';
                break;

            default:
                break;
        }
        
        var entityArray = [];
        var promises = [];

        _.each(ids, function (id) {            
            if(id) {
                promises.push(archetypeCacheService.getEntityByUmbracoId(id, type).then(function(entity) {
                    if(entity) {
                        entityArray.push(entity.name);
                    }
                }).promise);
            }
        });
        
        $q.all(promises).then(function() {           
            if(entityArray.length == 0) {
                deferred.resolve("");
            }
            
            var firstEntityName = entityArray[0];
            
            var value = firstEntityName + (ids.length > 1 ? ", ..." : "");
            
            deferred.resolve(value);
        });
                
        return deferred.promise;
    }

    function coreMediaPicker(value, scope, args) {
        var deferred = $q.defer();
        
        if(value) {
            archetypeCacheService.getEntityById(value, "media").then(function(entity) {
                deferred.resolve(entity.name);
            });
        }
        else
        {
            deferred.resolve("");
        }

        return deferred.promise;
    }

    function coreMediaPickerV2(value, scope, args) {
        var deferred = $q.defer();
       
        //console.log("value=");
        //console.log(value);       
       
        if(value) {
            archetypeCacheService.getEntityByUmbracoId(value, "media").then(function(entity) {
                deferred.resolve(entity.name);
            });
        }
        else
        {
            deferred.resolve("");
        }

        return deferred.promise;
    }

    function coreContentPicker(value, scope, args) {
        var deferred = $q.defer();
        
        if(value) {
            archetypeCacheService.getEntityById(value, "document").then(function(entity) {
                deferred.resolve(entity.name);
            });
        }
        else 
        {
            deferred.resolve("");
        }
        
        return deferred.promise;
    }

    function coreContentPickerV2(value, scope, args) {
        var deferred = $q.defer();
      
        if (value) {
            archetypeCacheService.getEntityByUmbracoId(value, "document").then(function(entity) {
                deferred.resolve(entity.name);
            });
        }
        else 
        {
            deferred.resolve("");
        }

        return deferred.promise;
    }

    function imulusUrlPicker(value, scope, args) {
        var deferred = $q.defer();        
        
        if(!value) {
            deferred.resolve("");
            
            return deferred.promise;
        }
        
        if(!args.propertyName) {
            args = {propertyName: "name"}
        }
        
        if(typeof(value) != "object") {
            value = JSON.parse(value);
        }

        //console.log("urlpicker value...");
        //console.log(value);
        
        if(value.length) {
            value = value[0];
        }       
               
        switch (value.type) {
            case "content":
                if(value.typeData.contentId) {
                    archetypeCacheService.getEntityById(value.typeData.contentId, "Document").then(function(entity) {
                        //console.log("Retrived entity from cache!");
                        //console.log("Resolving the entity name with " + entity[args.propertyName]);
                        deferred.resolve(entity[args.propertyName]);
                    });
                }
                break;

            case "media":
                if(value.typeData.mediaId) {
                    archetypeCacheService.getEntityById(value.typeData.mediaId, "Media").then(function(entity) {
                        deferred.resolve(entity[args.propertyName]);
                    });
                }
                break;

            case "url":
                deferred.resolve(value.typeData.url);
                
                return deferred.promise;
                
            default:
                break;
        }
        
        return deferred.promise;
    }

    function coreTinyMce(value, scope, args) {
        if(!args.contentLength) {
            args = {contentLength: 160}
        }

        var suffix = "";
        var strippedText = $("<div/>").html(value).text();

        if(strippedText.length > args.contentLength) {
            suffix = "…";
        }

        return strippedText.substring(0, args.contentLength) + suffix;
    }

    function rjpMultiUrlPicker(values, scope, args) {
        var names = [];

        _.each(values, function (value) {
            if (value.name) {
                names.push(value.name);
            }
        });

        return names.join(", ");
    }
    
    return {
        getFieldsetTitle: function(scope, fieldsetConfigModel, fieldsetIndex) {

            if(!fieldsetConfigModel)
                return $q.when("");

            var fieldset = scope.model.value.fieldsets[fieldsetIndex];
            var fieldsetConfig = scope.getConfigFieldsetByAlias(fieldset.alias);
            var template = fieldsetConfigModel.labelTemplate;
            var promises = [];

            if (template.length < 1)
                return $q.when(fieldsetConfig.label);

            var rgx = /{{.*?}}/g;
            var matches = splitByRegex(rgx, template);

            _.each(matches, function (match) {

                // Skip over substrings that didn't match the regex (they do not require a transformation).
                if (!match.isMatch) {
                    return;
                }

                // split the template in case it consists of multiple property aliases and/or functions
                var templates = match.value.replace("{{", '').replace("}}", '').split("|");
                var templateLabelValue = "";

                for(var i = 0; i < templates.length; i++) {
                    // stop looking for a template label value if a previous template part already yielded a value
                    if(templateLabelValue !== "") {
                        break;
                    }
                    
                    var template = templates[i];
                    
                    //test for function
                    var beginParamsIndexOf = template.indexOf("(");
                    var endParamsIndexOf = template.indexOf(")");

                    //if passed a function
                    if(beginParamsIndexOf !== -1 && endParamsIndexOf !== -1)
                    {
                        var functionName = template.substring(0, beginParamsIndexOf);
                        var propertyAlias = template.substring(beginParamsIndexOf + 1, endParamsIndexOf).split(',')[0];

                        var args = {};

                        var beginArgsIndexOf = template.indexOf(',');

                        if(beginArgsIndexOf !== -1) {

                            var argsString = template.substring(beginArgsIndexOf + 1, endParamsIndexOf).trim();

                            var normalizedJsonString = argsString.replace(/(\w+)\s*:/g, '"$1":');

                            args = JSON.parse(normalizedJsonString);
                        }
                       
                        templateLabelValue = executeFunctionByName(functionName, window, scope.getPropertyValueByAlias(fieldset, propertyAlias), scope, args);
                    }
                    //normal {{foo}} syntax
                    else {
                        propertyAlias = template;
                        
                        var rawValue = scope.getPropertyValueByAlias(fieldset, propertyAlias);

                        templateLabelValue = rawValue;

                        //determine the type of editor
                        var propertyConfig = _.find(fieldsetConfigModel.properties, function(property){
                            return property.alias === propertyAlias;
                        });

                        if(propertyConfig) {
                            var datatype = archetypeCacheService.getDatatypeByGuid(propertyConfig.dataTypeGuid);

                            if(datatype) {
                                //try to get built-in label
                                var label = getNativeLabel(datatype, rawValue, scope);
                                
                                if (label) {
                                    templateLabelValue = label;
                                }
                            }
                        }
                    }
                }

                // Process the value (i.e., reduce any functions or promises down to strings).
                processLabelValue(templateLabelValue, promises, match);
            });

            // Wait for all of the promises to resolve before constructing the full fieldset label.
            return repeatedlyWaitForPromises(promises).then(function () {

                //console.log("done waiting...")
                // Extract string values and combine them into a single string.
                var substrings = _.map(matches, function (value) {
                    return value.value;
                });
                
                var combinedSubstrings = substrings.join('');

                //console.log(combinedSubstrings);
                
                // Return the title.
                return combinedSubstrings;
            });
        }
    }
});
angular.module('umbraco.services').factory('archetypeCacheService', function (archetypePropertyEditorResource, $q, entityResource) {
    //private
    var entityCache = [];
    var datatypeCache = [];
    
    return {
        initialize: function() {
            return archetypePropertyEditorResource.getAllDataTypesForCache().then(function(data) {
                _.each(data, function(datatype) {
                    datatypeCache[datatype.dataTypeGuid] = datatype;
                });
            });
        },
        
        getDataTypeFromCache: function(guid) {
            return datatypeCache[guid];
        },
 
        getDatatypeByGuid: function(guid) {
            var cachedDatatype = this.getDataTypeFromCache(guid);
            
            if(cachedDatatype) {
                return cachedDatatype;
            }

            return null;
        },

        getEntityById: function(id, type) {
            var deferred = $q.defer();
            
            //console.log(entityCache);
                
            var cachedEntity = entityCache[id];

            if(cachedEntity) {
                //console.log("Found ID " + id);
                
                deferred.resolve(cachedEntity);
                
                return deferred.promise;
            }

            //go get it from server
            entityResource.getById(id, type).then(function(entity) {
                entityCache[id] = entity;
                
                //console.log("entity ID is now resolved into cache...");
                //console.log(entityCache);
                
                deferred.resolve(entity);
            });

            return deferred.promise;
        },

        //perhaps this should return a promise?
        getEntityByUmbracoId: function(udi, type) {
            var deferred = $q.defer();
            
            var cachedEntity = entityCache[udi];

            if(cachedEntity) {
                //console.log("Found UDI " + udi);
                
                deferred.resolve(cachedEntity);
                
                return deferred.promise;
            }

            //go get it from server
            entityResource.getByIds([udi], type).then(function (entities) {
                // prevent infinite lookups with a default entity
                var entity = entities.length > 0 ? entities[0] : { udi: udi, name: "" };

                entityCache[udi] = entity;
                
                //console.log("entity UDI is now resolved into cache...");
                //console.log(entityCache);

                deferred.resolve(entity);
            });

            return deferred.promise;
        }
    }
});
angular.module('umbraco.services').factory('archetypeGlobalConfigService', function ($q, $http, umbRequestHelper) {
    return {
        globalSettings: function () {
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/globalSettings"), 'Failed to get whether or not we are checking for updates!'
            );
        },
        setCheckForUpdates: function (isChecking) {
            return umbRequestHelper.resourcePromise(
                $http.post("backoffice/ArchetypeApi/ArchetypeDataType/SetCheckForUpdates", isChecking), 'Failed to update check status!'
            );
        },
        checkForUpdates: function () {
            return umbRequestHelper.resourcePromise(
                $http.post("backoffice/ArchetypeApi/ArchetypeDataType/checkForUpdates", { }), 'Failed to check for updates!'
            );
        }
    }
});