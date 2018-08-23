﻿// <copyright file="StringExtensions.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace UmbMapper.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="string"/>.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Gets Ids from known XML fragments (as saved by the MNTP / XPath CheckBoxList)
        /// </summary>
        /// <param name="xml">The Xml</param>
        /// <returns>An array of node ids as integer.</returns>
        public static int[] GetXmlIds(this string xml)
        {
            var ids = new List<int>();

            if (!string.IsNullOrEmpty(xml))
            {
                using (var xmlReader = XmlReader.Create(new StringReader(xml)))
                {
                    try
                    {
                        xmlReader.Read();

                        // Check name of first element
                        switch (xmlReader.Name)
                        {
                            case "MultiNodePicker":
                            case "XPathCheckBoxList":
                            case "CheckBoxTree":

                                // Position on first <nodeId>
                                xmlReader.ReadStartElement();

                                while (!xmlReader.EOF)
                                {
                                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "nodeId")
                                    {
                                        if (int.TryParse(xmlReader.ReadElementContentAsString(), out int id))
                                        {
                                            ids.Add(id);
                                        }
                                    }
                                    else
                                    {
                                        // Step the reader on
                                        xmlReader.Read();
                                    }
                                }

                                break;
                        }
                    }
                    catch
                    {
                        // Failed to read as Xml
                    }
                }
            }

            return ids.ToArray();
        }
    }
}
