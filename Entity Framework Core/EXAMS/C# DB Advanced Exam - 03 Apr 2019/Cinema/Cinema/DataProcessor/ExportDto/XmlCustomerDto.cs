using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ExportDto
{
    [XmlType("Customer")]
    public class XmlCustomerDto
    {
        [XmlAttribute("FirstName")]
        public string FirstName { get; set; }

        [XmlAttribute("LastName")]
        public string LastName { get; set; }

        [XmlElement("SpentMoney")]
        public decimal SpentMoney { get; set; }

        [XmlElement("SpentTime")]
        public string SpentTime { get; set; }
    }
}
