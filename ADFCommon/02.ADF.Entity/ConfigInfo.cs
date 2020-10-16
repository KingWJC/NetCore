using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ADF.Entity
{
    [Table("JHICU_CONFIG_TEMPLATE")]
    public class ConfigInfo
    {
        [Key, Column(Order = 1)]
        public Int32 ID { get; set; }
        public Byte[] Data { get; set; }
    }
}