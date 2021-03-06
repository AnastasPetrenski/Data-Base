﻿using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.ExportUserProducts
{
    [XmlRoot("users")]
    [XmlType("User")]
    public class ExportUserDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public ExportProductCountDTO SoldProducts { get; set; }
    }
}
