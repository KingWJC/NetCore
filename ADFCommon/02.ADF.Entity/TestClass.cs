using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ADF.Entity
{
    [Table("JHZM_PAT_MAIN")]
    public class JHZM_PAT_MAIN
    {
        private int _pat_main_id;
        /// <summary>
        /// 患者基础信息主索引
        /// </summary>
        [Key, Column(Order = 1)]
        public int PAT_MAIN_ID
        {
            get { return _pat_main_id; }
            set { _pat_main_id = value; }
        }

        private string _out_patient_no;
        /// <summary>
        /// 就诊卡号(门诊号)
        /// </summary>
        public string OUT_PATIENT_NO
        {
            get { return _out_patient_no; }
            set { _out_patient_no = value; }
        }

        private string _hospital_no;
        /// <summary>
        /// 医院编码
        /// </summary>
        public string HOSPITAL_NO
        {
            get { return _hospital_no; }
            set { _hospital_no = value; }
        }

        private string _patient_name;
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PATIENT_NAME
        {
            get { return _patient_name; }
            set { _patient_name = value; }
        }

        private string _sex;
        /// <summary>
        /// 患者性别
        /// </summary>
        public string SEX
        {
            get { return _sex; }
            set { _sex = value; }
        }

        private int _age;
        /// <summary>
        /// 患者年龄
        /// </summary>
        public int AGE
        {
            get { return _age; }
            set { _age = value; }
        }

        private string _service_agency;
        /// <summary>
        /// 工作单位
        /// </summary>
        public string SERVICE_AGENCY
        {
            get { return _service_agency; }
            set { _service_agency = value; }
        }

        private string _phonenumber;
        /// <summary>
        /// 电话号码
        /// </summary>
        public string PHONENUMBER
        {
            get { return _phonenumber; }
            set { _phonenumber = value; }
        }

        private string _visit_dept;
        /// <summary>
        /// 就诊科室
        /// </summary>
        public string VISIT_DEPT
        {
            get { return _visit_dept; }
            set { _visit_dept = value; }
        }

        private DateTime _visit_date;
        /// <summary>
        /// 就诊时间
        /// </summary>
        public DateTime VISIT_DATE
        {
            get { return _visit_date; }
            set { _visit_date = value; }
        }

        private int _insurance_no;
        /// <summary>
        /// 患者 医保号 号码
        /// </summary>
        public int INSURANCE_NO
        {
            get { return _insurance_no; }
            set { _insurance_no = value; }
        }

        private DateTime _report_datetime;
        /// <summary>
        /// 记录日期时间
        /// </summary>
        public DateTime REPORT_DATETIME
        {
            get { return _report_datetime; }
            set { _report_datetime = value; }
        }

        private DateTime _onset_datetime;
        /// <summary>
        /// 发病日期时间
        /// </summary>
        public DateTime ONSET_DATETIME
        {
            get { return _onset_datetime; }
            set { _onset_datetime = value; }
        }

        private int _onset_time;
        /// <summary>
        /// 发病至就诊 时间 (H) 时长
        /// </summary>
        public int ONSET_TIME
        {
            get { return _onset_time; }
            set { _onset_time = value; }
        }

        private string _causes;
        /// <summary>
        /// 病因 描述 说明
        /// </summary>
        public string CAUSES
        {
            get { return _causes; }
            set { _causes = value; }
        }

        private string _symptom;
        /// <summary>
        /// 症状描述
        /// </summary>
        public string SYMPTOM
        {
            get { return _symptom; }
            set { _symptom = value; }
        }

        private string _concomitant_symptoms;
        /// <summary>
        /// 伴随症状 描述 说明
        /// </summary>
        public string CONCOMITANT_SYMPTOMS
        {
            get { return _concomitant_symptoms; }
            set { _concomitant_symptoms = value; }
        }

        private string _zzhj;
        /// <summary>
        /// 症状 缓解情况 说明
        /// </summary>
        public string ZZHJ
        {
            get { return _zzhj; }
            set { _zzhj = value; }
        }

        private string _yylb;
        /// <summary>
        /// 发病后用药 类别 代码
        /// </summary>
        public string YYLB
        {
            get { return _yylb; }
            set { _yylb = value; }
        }

        private string _aspl;
        /// <summary>
        /// 阿司匹林 发生 标志
        /// </summary>
        public string ASPL
        {
            get { return _aspl; }
            set { _aspl = value; }
        }

        private string _lbgl;
        /// <summary>
        /// 氯吡格雷 发生 标志
        /// </summary>
        public string LBGL
        {
            get { return _lbgl; }
            set { _lbgl = value; }
        }

        private string _rsyw;
        /// <summary>
        /// 溶栓药物 发生 标志
        /// </summary>
        public string RSYW
        {
            get { return _rsyw; }
            set { _rsyw = value; }
        }

        private string _knyw;
        /// <summary>
        /// 抗凝药物 发生 标志
        /// </summary>
        public string KNYW
        {
            get { return _knyw; }
            set { _knyw = value; }
        }

        private string _fngslyw;
        /// <summary>
        /// 非脑梗死类药物 用药 说明
        /// </summary>
        public string FNGSLYW
        {
            get { return _fngslyw; }
            set { _fngslyw = value; }
        }

        private string _jwjb;
        /// <summary>
        /// 既往疾病 情况 代码
        /// </summary>
        public string JWJB
        {
            get { return _jwjb; }
            set { _jwjb = value; }
        }

        private string _fbqclgrsw;
        /// <summary>
        /// 发病前处理个人事务 情况 代码
        /// </summary>
        public string FBQCLGRSW
        {
            get { return _fbqclgrsw; }
            set { _fbqclgrsw = value; }
        }

        private string _dlxz;
        /// <summary>
        /// 独立行走 发生 标志
        /// </summary>
        public string DLXZ
        {
            get { return _dlxz; }
            set { _dlxz = value; }
        }

        private string _xhdcx;
        /// <summary>
        /// 消化道出血史 发生 标志
        /// </summary>
        public string XHDCX
        {
            get { return _xhdcx; }
            set { _xhdcx = value; }
        }

        private string _xhdcx_datetime;
        /// <summary>
        /// 消化道出血 发生时间 代码
        /// </summary>
        public string XHDCX_DATETIME
        {
            get { return _xhdcx_datetime; }
            set { _xhdcx_datetime = value; }
        }

        private string _yzwss;
        /// <summary>
        /// 严重外伤史 发生 标志
        /// </summary>
        public string YZWSS
        {
            get { return _yzwss; }
            set { _yzwss = value; }
        }

        private string _yzwss_datetime;
        /// <summary>
        /// 严重外伤史 发生时间 代码
        /// </summary>
        public string YZWSS_DATETIME
        {
            get { return _yzwss_datetime; }
            set { _yzwss_datetime = value; }
        }

        private string _dsss;
        /// <summary>
        /// 大手术史 发生 标志
        /// </summary>
        public string DSSS
        {
            get { return _dsss; }
            set { _dsss = value; }
        }

        private string _dsss_datetime;
        /// <summary>
        /// 大手术史 手术时间 代码
        /// </summary>
        public string DSSS_DATETIME
        {
            get { return _dsss_datetime; }
            set { _dsss_datetime = value; }
        }

        private string _xj;
        /// <summary>
        /// 酗酒 发生 标志
        /// </summary>
        public string XJ
        {
            get { return _xj; }
            set { _xj = value; }
        }

        private string _ywgm;
        /// <summary>
        /// 药物过敏史 发生 标志
        /// </summary>
        public string YWGM
        {
            get { return _ywgm; }
            set { _ywgm = value; }
        }

        private string _ywgmmc;
        /// <summary>
        /// 药物过敏史 药物 名称
        /// </summary>
        public string YWGMMC
        {
            get { return _ywgmmc; }
            set { _ywgmmc = value; }
        }

        private int _ccnl;
        /// <summary>
        /// 初潮年龄(岁)
        /// </summary>
        public int CCNL
        {
            get { return _ccnl; }
            set { _ccnl = value; }
        }

        private int _xjqts;
        /// <summary>
        /// 行经期天数
        /// </summary>
        public int XJQTS
        {
            get { return _xjqts; }
            set { _xjqts = value; }
        }

        private int _yjzq;
        /// <summary>
        /// 月经周期(D)
        /// </summary>
        public int YJZQ
        {
            get { return _yjzq; }
            set { _yjzq = value; }
        }

        private DateTime _mcyjrq;
        /// <summary>
        /// 末次月经日期
        /// </summary>
        public DateTime MCYJRQ
        {
            get { return _mcyjrq; }
            set { _mcyjrq = value; }
        }

        private int _yc;
        /// <summary>
        /// 孕次
        /// </summary>
        public int YC
        {
            get { return _yc; }
            set { _yc = value; }
        }

        private int _cc;
        /// <summary>
        /// 产次
        /// </summary>
        public int CC
        {
            get { return _cc; }
            set { _cc = value; }
        }

        private int _sbp;
        /// <summary>
        /// 收缩压(mmHg)
        /// </summary>
        public int SBP
        {
            get { return _sbp; }
            set { _sbp = value; }
        }

        private int _dbp;
        /// <summary>
        /// 舒张压(mmHg)
        /// </summary>
        public int DBP
        {
            get { return _dbp; }
            set { _dbp = value; }
        }

        private int _hr;
        /// <summary>
        /// 心率(次/min)
        /// </summary>
        public int HR
        {
            get { return _hr; }
            set { _hr = value; }
        }

        private string _tlct;
        /// <summary>
        /// 头颅CT 检查结果 说明
        /// </summary>
        public string TLCT
        {
            get { return _tlct; }
            set { _tlct = value; }
        }

        private string _ncxwz;
        /// <summary>
        /// 脑出血 位置 代码
        /// </summary>
        public string NCXWZ
        {
            get { return _ncxwz; }
            set { _ncxwz = value; }
        }

        private string _ncxxzl;
        /// <summary>
        /// 脑出血 血肿量 代码
        /// </summary>
        public string NCXXZL
        {
            get { return _ncxxzl; }
            set { _ncxxzl = value; }
        }

        private string _diagnosis;
        /// <summary>
        /// 诊断 名称 代码
        /// </summary>
        public string DIAGNOSIS
        {
            get { return _diagnosis; }
            set { _diagnosis = value; }
        }

        private int _abcd2score;
        /// <summary>
        /// TIA-ABCD2评分 总分 数值
        /// </summary>
        public int ABCD2SCORE
        {
            get { return _abcd2score; }
            set { _abcd2score = value; }
        }

        private int _gcsscore;
        /// <summary>
        /// GCS评分 总分
        /// </summary>
        public int GCSSCORE
        {
            get { return _gcsscore; }
            set { _gcsscore = value; }
        }

        private string _ywmc;
        /// <summary>
        /// 药物名称
        /// </summary>
        public string YWMC
        {
            get { return _ywmc; }
            set { _ywmc = value; }
        }

        private DateTime _first_treatment_datetime;
        /// <summary>
        /// 首次接诊 时间 日期时间
        /// </summary>
        public DateTime FIRST_TREATMENT_DATETIME
        {
            get { return _first_treatment_datetime; }
            set { _first_treatment_datetime = value; }
        }

        private DateTime _first_lab_datetime;
        /// <summary>
        /// 首次开立化验 时间 日期时间
        /// </summary>
        public DateTime FIRST_LAB_DATETIME
        {
            get { return _first_lab_datetime; }
            set { _first_lab_datetime = value; }
        }

        private DateTime _first_ctmr_datetime;
        /// <summary>
        /// 首次开立CT或MR 时间 日期时间
        /// </summary>
        public DateTime FIRST_CTMR_DATETIME
        {
            get { return _first_ctmr_datetime; }
            set { _first_ctmr_datetime = value; }
        }

        private string _patient_whereabouts;
        /// <summary>
        /// 患者 去向 说明
        /// </summary>
        public string PATIENT_WHEREABOUTS
        {
            get { return _patient_whereabouts; }
            set { _patient_whereabouts = value; }
        }

        private string _jcjy;
        /// <summary>
        /// 检查/检验结果
        /// </summary>
        public string JCJY
        {
            get { return _jcjy; }
            set { _jcjy = value; }
        }

        private int _nihssscore;
        /// <summary>
        /// NIHSS评分 分数 数值
        /// </summary>
        public int NIHSSSCORE
        {
            get { return _nihssscore; }
            set { _nihssscore = value; }
        }

        private string _nihssbc;
        /// <summary>
        /// 脑梗死或短暂性脑缺血发作病史 补充内容 说明
        /// </summary>
        public string NIHSSBC
        {
            get { return _nihssbc; }
            set { _nihssbc = value; }
        }

        private string _nihssyssp;
        /// <summary>
        /// NIHSS评分 意识水平 代码
        /// </summary>
        public string NIHSSYSSP
        {
            get { return _nihssyssp; }
            set { _nihssyssp = value; }
        }

        private string _nihssysspzl;
        /// <summary>
        /// NIHSS评分 意识水平指令 代码
        /// </summary>
        public string NIHSSYSSPZL
        {
            get { return _nihssysspzl; }
            set { _nihssysspzl = value; }
        }

        private string _nihssns;
        /// <summary>
        /// NIHSS评分 凝视 代码
        /// </summary>
        public string NIHSSNS
        {
            get { return _nihssns; }
            set { _nihssns = value; }
        }

        private string _nihsssy;
        /// <summary>
        /// NIHSS评分 视野 代码
        /// </summary>
        public string NIHSSSY
        {
            get { return _nihsssy; }
            set { _nihsssy = value; }
        }

        private string _nihssmt;
        /// <summary>
        /// NIHSS评分 面瘫 代码
        /// </summary>
        public string NIHSSMT
        {
            get { return _nihssmt; }
            set { _nihssmt = value; }
        }

        private string _nihsszszyd;
        /// <summary>
        /// NIHSS评分 左上肢运动 代码
        /// </summary>
        public string NIHSSZSZYD
        {
            get { return _nihsszszyd; }
            set { _nihsszszyd = value; }
        }

        private string _nihssyszyd;
        /// <summary>
        /// NIHSS评分 右上肢运动 代码
        /// </summary>
        public string NIHSSYSZYD
        {
            get { return _nihssyszyd; }
            set { _nihssyszyd = value; }
        }

        private string _nihsszxzyd;
        /// <summary>
        /// NIHSS评分 左下肢运动 代码
        /// </summary>
        public string NIHSSZXZYD
        {
            get { return _nihsszxzyd; }
            set { _nihsszxzyd = value; }
        }

        private string _nihssyxzyd;
        /// <summary>
        /// NIHSS评分 右下肢运动 代码
        /// </summary>
        public string NIHSSYXZYD
        {
            get { return _nihssyxzyd; }
            set { _nihssyxzyd = value; }
        }

        private string _nihssgjst;
        /// <summary>
        /// NIHSS评分 共济失调 代码
        /// </summary>
        public string NIHSSGJST
        {
            get { return _nihssgjst; }
            set { _nihssgjst = value; }
        }

        private string _nihssgj;
        /// <summary>
        /// NIHSS评分 感觉 代码
        /// </summary>
        public string NIHSSGJ
        {
            get { return _nihssgj; }
            set { _nihssgj = value; }
        }

        private string _nihssyy;
        /// <summary>
        /// NIHSS评分 语言 代码
        /// </summary>
        public string NIHSSYY
        {
            get { return _nihssyy; }
            set { _nihssyy = value; }
        }

        private string _nihssgyza;
        /// <summary>
        /// NIHSS评分 构音障碍 代码
        /// </summary>
        public string NIHSSGYZA
        {
            get { return _nihssgyza; }
            set { _nihssgyza = value; }
        }

        private string _nihsshsz;
        /// <summary>
        /// NIHSS评分 忽视症 代码
        /// </summary>
        public string NIHSSHSZ
        {
            get { return _nihsshsz; }
            set { _nihsshsz = value; }
        }

        private string _abcd2nl;
        /// <summary>
        /// TIA-ABCD2评分 年龄分级 代码
        /// </summary>
        public string ABCD2NL
        {
            get { return _abcd2nl; }
            set { _abcd2nl = value; }
        }

        private string _abcd2gxy;
        /// <summary>
        /// TIA-ABCD2评分高血压 发生 标志
        /// </summary>
        public string ABCD2GXY
        {
            get { return _abcd2gxy; }
            set { _abcd2gxy = value; }
        }

        private string _abcd2zzfj;
        /// <summary>
        /// TIA-ABCD2评分 症状分级 代码
        /// </summary>
        public string ABCD2ZZFJ
        {
            get { return _abcd2zzfj; }
            set { _abcd2zzfj = value; }
        }

        private string _abcd2zzcxsj;
        /// <summary>
        /// TIA-ABCD2评分 症状持续时间 代码
        /// </summary>
        public string ABCD2ZZCXSJ
        {
            get { return _abcd2zzcxsj; }
            set { _abcd2zzcxsj = value; }
        }

        private string _abcd2tnb;
        /// <summary>
        /// TIA-ABCD2评分 糖尿病 发生 标志
        /// </summary>
        public string ABCD2TNB
        {
            get { return _abcd2tnb; }
            set { _abcd2tnb = value; }
        }

        private string _gcsyd;
        /// <summary>
        /// GCS评分 运动 代码
        /// </summary>
        public string GCSYD
        {
            get { return _gcsyd; }
            set { _gcsyd = value; }
        }

        private string _gcsyy;
        /// <summary>
        /// GCS评分 语言 代码
        /// </summary>
        public string GCSYY
        {
            get { return _gcsyy; }
            set { _gcsyy = value; }
        }

        private string _gcswyyqk;
        /// <summary>
        /// GCS评分 无语言情况 代码
        /// </summary>
        public string GCSWYYQK
        {
            get { return _gcswyyqk; }
            set { _gcswyyqk = value; }
        }

        private string _gcszy;
        /// <summary>
        /// GCS评分 睁眼 代码
        /// </summary>
        public string GCSZY
        {
            get { return _gcszy; }
            set { _gcszy = value; }
        }

        private string _adress;
        /// <summary>
        /// 患者 现住址 名称
        /// </summary>
        public string ADRESS
        {
            get { return _adress; }
            set { _adress = value; }
        }

        private string _ncxbcsm;
        /// <summary>
        /// 脑出血 补充内容 说明
        /// </summary>
        public string NCXBCSM
        {
            get { return _ncxbcsm; }
            set { _ncxbcsm = value; }
        }

        private string _nihssyssptw;
        /// <summary>
        /// NIHSS评分 意识水平提问 代码
        /// </summary>
        public string NIHSSYSSPTW
        {
            get { return _nihssyssptw; }
            set { _nihssyssptw = value; }
        }

        private string _zszjz;
        /// <summary>
        /// 左上肢截肢或关节融合 解释 说明
        /// </summary>
        public string ZSZJZ
        {
            get { return _zszjz; }
            set { _zszjz = value; }
        }

        private string _yszjz;
        /// <summary>
        /// 右上肢截肢或关节融合 解释 说明
        /// </summary>
        public string YSZJZ
        {
            get { return _yszjz; }
            set { _yszjz = value; }
        }

        private string _zxzjz;
        /// <summary>
        /// 左下肢截肢或关节融合 解释 说明
        /// </summary>
        public string ZXZJZ
        {
            get { return _zxzjz; }
            set { _zxzjz = value; }
        }

        private string _yxzjz;
        /// <summary>
        /// 右下肢截肢或关节融合 解释 说明
        /// </summary>
        public string YXZJZ
        {
            get { return _yxzjz; }
            set { _yxzjz = value; }
        }

        private string _gyza;
        /// <summary>
        /// 构音障碍 气管插管或其他物理障碍 说明
        /// </summary>
        public string GYZA
        {
            get { return _gyza; }
            set { _gyza = value; }
        }

        private int _state;
        /// <summary>
        /// 有效状态
        /// </summary>
        public int STATE
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}