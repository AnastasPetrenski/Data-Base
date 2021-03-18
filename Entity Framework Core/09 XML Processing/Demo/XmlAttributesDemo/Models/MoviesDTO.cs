using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace XmlAttributesDemo.Models
{
    [XmlType("movie")]
    [Serializable]
    public class MoviesDTO
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("price")]
        public string Price { get; set; }

        [XmlIgnore]
        public string Date => DateTime.UtcNow.Date.ToString("f", CultureInfo.InvariantCulture);
    }
}
