using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ADF.Entity
{
    [Table("JHICU_DICT_MICDICT_DETAIL")]
    public class DictType
    {
        [Key, Column(Order = 1)]
        public long MICDICT_ID { get; set; }
        public int TYPE_ID { get; set; }
        public int DICT_ID { get; set; }
        public String DICT_NAME { get; set; }
        public String NOTE { get; set; }
        public String PYM { get; set; }
        public String WBM { get; set; }
        public String HOSPITAL_NO { get; set; }
        public int STATE { get; set; }

    }
}