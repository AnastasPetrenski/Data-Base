﻿using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Export
{
    public class XmlExportMostPopularDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("TotalMade")]
        public decimal TotalMade { get; set; }

        [XmlElement("TimesSold")]
        public int TimesSold { get; set; }
    }
}
