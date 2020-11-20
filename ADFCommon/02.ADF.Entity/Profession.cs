using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ADF.Entity
{
    [Table("JHICU_DICT_PROFESSION")]
    public class Profession
    {
        [Key,Column(Order=1)]
        public Int32 PROFESSION_ID { get; set; }
        public String PROFESSION_NAME { get; set; }
        public Int16 PROFESSION_TYPE { get; set; }
        public String PYM { get; set; }
        public String WBM { get; set; }
        public String HOSPITAL_NO { get; set; }
        public Int16 STATE { get; set; }
        public String DEPT_CODE { get; set; }
        public String PROFESSION_CONTEXT { get; set; }
        public Int16 TEMPLATE_ID{get;set;}
    }
}