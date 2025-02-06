using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PanelSemi_Coloradjustment
{
    internal partial class TotalProcess
    {
        public const UInt32 GAMMA_SIZE = 8192;     //8192 目前是 Primary 機種專用，如果要跟其他機種共用則需要加條件式區別
        public const UInt32 FPGA_MTP_SIZE = 0x2000;    //8192 目前是 Primary 機種專用，如果要跟其他機種共用則需要加條件式區別
        public const UInt32 GAMMA_MTP_SIZE = 8192;     //8192 目前是 Primary 機種專用，如果要跟其他機種共用則需要加條件式區別
        public bool flgDelFB;
        public bool isReadBack = true;

        public ListBox lstsvuiregadr = new ListBox();
        public ListBox lstget1 = new ListBox();
        public ListBox lstm = new ListBox();
        public ListBox lstmcuR60000 = new ListBox();
        public ListBox lstmcuW60000 = new ListBox();
        public ListBox lstmcuR66000 = new ListBox();
        public ListBox lstmcuW66000 = new ListBox();
        public List<string> svuiregadr = new List<string>(new string[GAMMA_SIZE / 4]);
        public string[] uiregadr_default = null;

        public static string[] uiregadr_default_p =
       {
            "0,MARK,128",
            "1,USR_GMA_EN,1",
            "2,USR_BRI_EN,1",
            "3,USR_CG_EN,1",
            "4,ENG_WT_PG_EN,0",
            "5,ENG_GMA_EN,0",
            "6,ENG_BRI_EN,0",
            "7,STATUS,1",
            "8,IDLE,0",
            "9,IDLE,0",
            "10,GMA1_USR,512",
            "11,GMA2_USR,1024",
            "12,GMA3_USR,2048",
            "13,GMA4_USR,4080",
            "14,CT_RED_USR,1024",
            "15,CT_GRN_USR,1024",
            "16,CT_BLU_USR,1024",
            "17,CG_MAT_A1,16384",
            "18,CG_MAT_A2,0",
            "19,CG_MAT_A3,0",
            "20,CG_MAT_A4,0",
            "21,CG_MAT_A5,16384",
            "22,CG_MAT_A6,0",
            "23,CG_MAT_A7,0",
            "24,CG_MAT_A8,0",
            "25,CG_MAT_A9,16384",
            "26,CG_RED_X,0",
            "27,CG_RED_Y,0",
            "28,CG_GRN_X,0",
            "29,CG_GRN_Y,0",
            "30,CG_BLU_X,0",
            "31,CG_BLU_Y,0",
            "32,CG_WHI_X,0",
            "33,CG_WHI_Y,0",
            "34,CG_CT_VAL,0",
            "35,GMA_VAL,220",
            "36,IDLE,0",
            "37,IDLE,0",
            "38,IDLE,0",
            "39,IDLE,0",
            "40,WT_PG_RED_ENG,4080",
            "41,WT_PG_GRN_ENG,4080",
            "42,WT_PG_BLU_ENG,4080",
            "43,CT_RED_ENG_XB1_1,1024",
            "44,CT_GRN_ENG_XB1_1,1024",
            "45,CT_BLU_ENG_XB1_1,1024",
            "46,CT_RED_ENG_XB1_2,1024",
            "47,CT_GRN_ENG_XB1_2,1024",
            "48,CT_BLU_ENG_XB1_2,1024",
            "49,CT_RED_ENG_XB1_3,1024",
            "50,CT_GRN_ENG_XB1_3,1024",
            "51,CT_BLU_ENG_XB1_3,1024",
            "52,CT_RED_ENG_XB1_4,1024",
            "53,CT_GRN_ENG_XB1_4,1024",
            "54,CT_BLU_ENG_XB1_4,1024",
            "55,CT_RED_ENG_XB2_1,1024",
            "56,CT_GRN_ENG_XB2_1,1024",
            "57,CT_BLU_ENG_XB2_1,1024",
            "58,CT_RED_ENG_XB2_2,1024",
            "59,CT_GRN_ENG_XB2_2,1024",
            "60,CT_BLU_ENG_XB2_2,1024",
            "61,CT_RED_ENG_XB2_3,1024",
            "62,CT_GRN_ENG_XB2_3,1024",
            "63,CT_BLU_ENG_XB2_3,1024",
            "64,CT_RED_ENG_XB2_4,1024",
            "65,CT_GRN_ENG_XB2_4,1024",
            "66,CT_BLU_ENG_XB2_4,1024",
            "67,CT_RED_ENG_XB3_1,1024",
            "68,CT_GRN_ENG_XB3_1,1024",
            "69,CT_BLU_ENG_XB3_1,1024",
            "70,CT_RED_ENG_XB3_2,1024",
            "71,CT_GRN_ENG_XB3_2,1024",
            "72,CT_BLU_ENG_XB3_2,1024",
            "73,CT_RED_ENG_XB3_3,1024",
            "74,CT_GRN_ENG_XB3_3,1024",
            "75,CT_BLU_ENG_XB3_3,1024",
            "76,CT_RED_ENG_XB3_4,1024",
            "77,CT_GRN_ENG_XB3_4,1024",
            "78,CT_BLU_ENG_XB3_4,1024",
            "79,CT_RED_ENG_XB4_1,1024",
            "80,CT_GRN_ENG_XB4_1,1024",
            "81,CT_BLU_ENG_XB4_1,1024",
            "82,CT_RED_ENG_XB4_2,1024",
            "83,CT_GRN_ENG_XB4_2,1024",
            "84,CT_BLU_ENG_XB4_2,1024",
            "85,CT_RED_ENG_XB4_3,1024",
            "86,CT_GRN_ENG_XB4_3,1024",
            "87,CT_BLU_ENG_XB4_3,1024",
            "88,CT_RED_ENG_XB4_4,1024",
            "89,CT_GRN_ENG_XB4_4,1024",
            "90,CT_BLU_ENG_XB4_4,1024",
            "91,WT_GMA1_RED_ENG_XB1_1,512",
            "92,WT_GMA2_RED_ENG_XB1_1,1024",
            "93,WT_GMA3_RED_ENG_XB1_1,2048",
            "94,WT_GMA4_RED_ENG_XB1_1,4080",
            "95,WT_GMA1_GRN_ENG_XB1_1,512",
            "96,WT_GMA2_GRN_ENG_XB1_1,1024",
            "97,WT_GMA3_GRN_ENG_XB1_1,2048",
            "98,WT_GMA4_GRN_ENG_XB1_1,4080",
            "99,WT_GMA1_BLU_ENG_XB1_1,512",
            "100,WT_GMA2_BLU_ENG_XB1_1,1024",
            "101,WT_GMA3_BLU_ENG_XB1_1,2048",
            "102,WT_GMA4_BLU_ENG_XB1_1,4080",
            "103,WT_GMA1_RED_ENG_XB1_2,512",
            "104,WT_GMA2_RED_ENG_XB1_2,1024",
            "105,WT_GMA3_RED_ENG_XB1_2,2048",
            "106,WT_GMA4_RED_ENG_XB1_2,4080",
            "107,WT_GMA1_GRN_ENG_XB1_2,512",
            "108,WT_GMA2_GRN_ENG_XB1_2,1024",
            "109,WT_GMA3_GRN_ENG_XB1_2,2048",
            "110,WT_GMA4_GRN_ENG_XB1_2,4080",
            "111,WT_GMA1_BLU_ENG_XB1_2,512",
            "112,WT_GMA2_BLU_ENG_XB1_2,1024",
            "113,WT_GMA3_BLU_ENG_XB1_2,2048",
            "114,WT_GMA4_BLU_ENG_XB1_2,4080",
            "115,WT_GMA1_RED_ENG_XB1_3,512",
            "116,WT_GMA2_RED_ENG_XB1_3,1024",
            "117,WT_GMA3_RED_ENG_XB1_3,2048",
            "118,WT_GMA4_RED_ENG_XB1_3,4080",
            "119,WT_GMA1_GRN_ENG_XB1_3,512",
            "120,WT_GMA2_GRN_ENG_XB1_3,1024",
            "121,WT_GMA3_GRN_ENG_XB1_3,2048",
            "122,WT_GMA4_GRN_ENG_XB1_3,4080",
            "123,WT_GMA1_BLU_ENG_XB1_3,512",
            "124,WT_GMA2_BLU_ENG_XB1_3,1024",
            "125,WT_GMA3_BLU_ENG_XB1_3,2048",
            "126,WT_GMA4_BLU_ENG_XB1_3,4080",
            "127,WT_GMA1_RED_ENG_XB1_4,512",
            "128,WT_GMA2_RED_ENG_XB1_4,1024",
            "129,WT_GMA3_RED_ENG_XB1_4,2048",
            "130,WT_GMA4_RED_ENG_XB1_4,4080",
            "131,WT_GMA1_GRN_ENG_XB1_4,512",
            "132,WT_GMA2_GRN_ENG_XB1_4,1024",
            "133,WT_GMA3_GRN_ENG_XB1_4,2048",
            "134,WT_GMA4_GRN_ENG_XB1_4,4080",
            "135,WT_GMA1_BLU_ENG_XB1_4,512",
            "136,WT_GMA2_BLU_ENG_XB1_4,1024",
            "137,WT_GMA3_BLU_ENG_XB1_4,2048",
            "138,WT_GMA4_BLU_ENG_XB1_4,4080",
            "139,WT_GMA1_RED_ENG_XB2_1,512",
            "140,WT_GMA2_RED_ENG_XB2_1,1024",
            "141,WT_GMA3_RED_ENG_XB2_1,2048",
            "142,WT_GMA4_RED_ENG_XB2_1,4080",
            "143,WT_GMA1_GRN_ENG_XB2_1,512",
            "144,WT_GMA2_GRN_ENG_XB2_1,1024",
            "145,WT_GMA3_GRN_ENG_XB2_1,2048",
            "146,WT_GMA4_GRN_ENG_XB2_1,4080",
            "147,WT_GMA1_BLU_ENG_XB2_1,512",
            "148,WT_GMA2_BLU_ENG_XB2_1,1024",
            "149,WT_GMA3_BLU_ENG_XB2_1,2048",
            "150,WT_GMA4_BLU_ENG_XB2_1,4080",
            "151,WT_GMA1_RED_ENG_XB2_2,512",
            "152,WT_GMA2_RED_ENG_XB2_2,1024",
            "153,WT_GMA3_RED_ENG_XB2_2,2048",
            "154,WT_GMA4_RED_ENG_XB2_2,4080",
            "155,WT_GMA1_GRN_ENG_XB2_2,512",
            "156,WT_GMA2_GRN_ENG_XB2_2,1024",
            "157,WT_GMA3_GRN_ENG_XB2_2,2048",
            "158,WT_GMA4_GRN_ENG_XB2_2,4080",
            "159,WT_GMA1_BLU_ENG_XB2_2,512",
            "160,WT_GMA2_BLU_ENG_XB2_2,1024",
            "161,WT_GMA3_BLU_ENG_XB2_2,2048",
            "162,WT_GMA4_BLU_ENG_XB2_2,4080",
            "163,WT_GMA1_RED_ENG_XB2_3,512",
            "164,WT_GMA2_RED_ENG_XB2_3,1024",
            "165,WT_GMA3_RED_ENG_XB2_3,2048",
            "166,WT_GMA4_RED_ENG_XB2_3,4080",
            "167,WT_GMA1_GRN_ENG_XB2_3,512",
            "168,WT_GMA2_GRN_ENG_XB2_3,1024",
            "169,WT_GMA3_GRN_ENG_XB2_3,2048",
            "170,WT_GMA4_GRN_ENG_XB2_3,4080",
            "171,WT_GMA1_BLU_ENG_XB2_3,512",
            "172,WT_GMA2_BLU_ENG_XB2_3,1024",
            "173,WT_GMA3_BLU_ENG_XB2_3,2048",
            "174,WT_GMA4_BLU_ENG_XB2_3,4080",
            "175,WT_GMA1_RED_ENG_XB2_4,512",
            "176,WT_GMA2_RED_ENG_XB2_4,1024",
            "177,WT_GMA3_RED_ENG_XB2_4,2048",
            "178,WT_GMA4_RED_ENG_XB2_4,4080",
            "179,WT_GMA1_GRN_ENG_XB2_4,512",
            "180,WT_GMA2_GRN_ENG_XB2_4,1024",
            "181,WT_GMA3_GRN_ENG_XB2_4,2048",
            "182,WT_GMA4_GRN_ENG_XB2_4,4080",
            "183,WT_GMA1_BLU_ENG_XB2_4,512",
            "184,WT_GMA2_BLU_ENG_XB2_4,1024",
            "185,WT_GMA3_BLU_ENG_XB2_4,2048",
            "186,WT_GMA4_BLU_ENG_XB2_4,4080",
            "187,WT_GMA1_RED_ENG_XB3_1,512",
            "188,WT_GMA2_RED_ENG_XB3_1,1024",
            "189,WT_GMA3_RED_ENG_XB3_1,2048",
            "190,WT_GMA4_RED_ENG_XB3_1,4080",
            "191,WT_GMA1_GRN_ENG_XB3_1,512",
            "192,WT_GMA2_GRN_ENG_XB3_1,1024",
            "193,WT_GMA3_GRN_ENG_XB3_1,2048",
            "194,WT_GMA4_GRN_ENG_XB3_1,4080",
            "195,WT_GMA1_BLU_ENG_XB3_1,512",
            "196,WT_GMA2_BLU_ENG_XB3_1,1024",
            "197,WT_GMA3_BLU_ENG_XB3_1,2048",
            "198,WT_GMA4_BLU_ENG_XB3_1,4080",
            "199,WT_GMA1_RED_ENG_XB3_2,512",
            "200,WT_GMA2_RED_ENG_XB3_2,1024",
            "201,WT_GMA3_RED_ENG_XB3_2,2048",
            "202,WT_GMA4_RED_ENG_XB3_2,4080",
            "203,WT_GMA1_GRN_ENG_XB3_2,512",
            "204,WT_GMA2_GRN_ENG_XB3_2,1024",
            "205,WT_GMA3_GRN_ENG_XB3_2,2048",
            "206,WT_GMA4_GRN_ENG_XB3_2,4080",
            "207,WT_GMA1_BLU_ENG_XB3_2,512",
            "208,WT_GMA2_BLU_ENG_XB3_2,1024",
            "209,WT_GMA3_BLU_ENG_XB3_2,2048",
            "210,WT_GMA4_BLU_ENG_XB3_2,4080",
            "211,WT_GMA1_RED_ENG_XB3_3,512",
            "212,WT_GMA2_RED_ENG_XB3_3,1024",
            "213,WT_GMA3_RED_ENG_XB3_3,2048",
            "214,WT_GMA4_RED_ENG_XB3_3,4080",
            "215,WT_GMA1_GRN_ENG_XB3_3,512",
            "216,WT_GMA2_GRN_ENG_XB3_3,1024",
            "217,WT_GMA3_GRN_ENG_XB3_3,2048",
            "218,WT_GMA4_GRN_ENG_XB3_3,4080",
            "219,WT_GMA1_BLU_ENG_XB3_3,512",
            "220,WT_GMA2_BLU_ENG_XB3_3,1024",
            "221,WT_GMA3_BLU_ENG_XB3_3,2048",
            "222,WT_GMA4_BLU_ENG_XB3_3,4080",
            "223,WT_GMA1_RED_ENG_XB3_4,512",
            "224,WT_GMA2_RED_ENG_XB3_4,1024",
            "225,WT_GMA3_RED_ENG_XB3_4,2048",
            "226,WT_GMA4_RED_ENG_XB3_4,4080",
            "227,WT_GMA1_GRN_ENG_XB3_4,512",
            "228,WT_GMA2_GRN_ENG_XB3_4,1024",
            "229,WT_GMA3_GRN_ENG_XB3_4,2048",
            "230,WT_GMA4_GRN_ENG_XB3_4,4080",
            "231,WT_GMA1_BLU_ENG_XB3_4,512",
            "232,WT_GMA2_BLU_ENG_XB3_4,1024",
            "233,WT_GMA3_BLU_ENG_XB3_4,2048",
            "234,WT_GMA4_BLU_ENG_XB3_4,4080",
            "235,WT_GMA1_RED_ENG_XB4_1,512",
            "236,WT_GMA2_RED_ENG_XB4_1,1024",
            "237,WT_GMA3_RED_ENG_XB4_1,2048",
            "238,WT_GMA4_RED_ENG_XB4_1,4080",
            "239,WT_GMA1_GRN_ENG_XB4_1,512",
            "240,WT_GMA2_GRN_ENG_XB4_1,1024",
            "241,WT_GMA3_GRN_ENG_XB4_1,2048",
            "242,WT_GMA4_GRN_ENG_XB4_1,4080",
            "243,WT_GMA1_BLU_ENG_XB4_1,512",
            "244,WT_GMA2_BLU_ENG_XB4_1,1024",
            "245,WT_GMA3_BLU_ENG_XB4_1,2048",
            "246,WT_GMA4_BLU_ENG_XB4_1,4080",
            "247,WT_GMA1_RED_ENG_XB4_2,512",
            "248,WT_GMA2_RED_ENG_XB4_2,1024",
            "249,WT_GMA3_RED_ENG_XB4_2,2048",
            "250,WT_GMA4_RED_ENG_XB4_2,4080",
            "251,WT_GMA1_GRN_ENG_XB4_2,512",
            "252,WT_GMA2_GRN_ENG_XB4_2,1024",
            "253,WT_GMA3_GRN_ENG_XB4_2,2048",
            "254,WT_GMA4_GRN_ENG_XB4_2,4080",
            "255,WT_GMA1_BLU_ENG_XB4_2,512",
            "256,WT_GMA2_BLU_ENG_XB4_2,1024",
            "257,WT_GMA3_BLU_ENG_XB4_2,2048",
            "258,WT_GMA4_BLU_ENG_XB4_2,4080",
            "259,WT_GMA1_RED_ENG_XB4_3,512",
            "260,WT_GMA2_RED_ENG_XB4_3,1024",
            "261,WT_GMA3_RED_ENG_XB4_3,2048",
            "262,WT_GMA4_RED_ENG_XB4_3,4080",
            "263,WT_GMA1_GRN_ENG_XB4_3,512",
            "264,WT_GMA2_GRN_ENG_XB4_3,1024",
            "265,WT_GMA3_GRN_ENG_XB4_3,2048",
            "266,WT_GMA4_GRN_ENG_XB4_3,4080",
            "267,WT_GMA1_BLU_ENG_XB4_3,512",
            "268,WT_GMA2_BLU_ENG_XB4_3,1024",
            "269,WT_GMA3_BLU_ENG_XB4_3,2048",
            "270,WT_GMA4_BLU_ENG_XB4_3,4080",
            "271,WT_GMA1_RED_ENG_XB4_4,512",
            "272,WT_GMA2_RED_ENG_XB4_4,1024",
            "273,WT_GMA3_RED_ENG_XB4_4,2048",
            "274,WT_GMA4_RED_ENG_XB4_4,4080",
            "275,WT_GMA1_GRN_ENG_XB4_4,512",
            "276,WT_GMA2_GRN_ENG_XB4_4,1024",
            "277,WT_GMA3_GRN_ENG_XB4_4,2048",
            "278,WT_GMA4_GRN_ENG_XB4_4,4080",
            "279,WT_GMA1_BLU_ENG_XB4_4,512",
            "280,WT_GMA2_BLU_ENG_XB4_4,1024",
            "281,WT_GMA3_BLU_ENG_XB4_4,2048",
            "282,WT_GMA4_BLU_ENG_XB4_4,4080",
            "283,IDLE,0",
            "284,IDLE,0",
            "285,IDLE,0",
            "286,IDLE,0",
            "287,IDLE,0",
            "288,IDLE,0",
            "289,IDLE,0",
            "290,IDLE,0",
            "291,IDLE,0",
            "292,IDLE,0",
            "293,IDLE,0",
            "294,IDLE,0",
            "295,IDLE,0",
            "296,IDLE,0",
            "297,IDLE,0",
            "298,IDLE,0",
            "299,IDLE,0",
            "300,GMA1_USR,512",
            "301,GMA2_USR,1024",
            "302,GMA3_USR,2048",
            "303,GMA4_USR,4080",
            "304,CT_RED_USR,1024",
            "305,CT_GRN_USR,1024",
            "306,CT_BLU_USR,1024",
            "307,CG_MAT_A1,16384",
            "308,CG_MAT_A2,0",
            "309,CG_MAT_A3,0",
            "310,CG_MAT_A4,0",
            "311,CG_MAT_A5,16384",
            "312,CG_MAT_A6,0",
            "313,CG_MAT_A7,0",
            "314,CG_MAT_A8,0",
            "315,CG_MAT_A9,16384",
            "316,CG_RED_X,0",
            "317,CG_RED_X,0",
            "318,CG_RED_X,0",
            "319,CG_RED_X,16384",
            "320,CG_RED_X,0",
            "321,CG_RED_Y,0",
            "322,CG_WHI_X,0",
            "323,CG_WHI_Y,0",
            "324,CG_CT_VAL,0",
            "325,GMA_VAL,0",
            "326,IDLE,0",
            "327,IDLE,0",
            "328,IDLE,0",
            "329,IDLE,0",
            "330,WT_PG_RED_ENG,4080",
            "331,WT_PG_GRN_ENG,4080",
            "332,WT_PG_BLU_ENG,4080",
            "333,CT_RED_ENG_XB1_1,1024",
            "334,CT_GRN_ENG_XB1_1,1024",
            "335,CT_BLU_ENG_XB1_1,1024",
            "336,CT_RED_ENG_XB1_2,1024",
            "337,CT_GRN_ENG_XB1_2,1024",
            "338,CT_BLU_ENG_XB1_2,1024",
            "339,CT_RED_ENG_XB1_3,1024",
            "340,CT_GRN_ENG_XB1_3,1024",
            "341,CT_BLU_ENG_XB1_3,1024",
            "342,CT_RED_ENG_XB1_4,1024",
            "343,CT_GRN_ENG_XB1_4,1024",
            "344,CT_BLU_ENG_XB1_4,1024",
            "345,CT_RED_ENG_XB2_1,1024",
            "346,CT_GRN_ENG_XB2_1,1024",
            "347,CT_BLU_ENG_XB2_1,1024",
            "348,CT_RED_ENG_XB2_2,1024",
            "349,CT_GRN_ENG_XB2_2,1024",
            "350,CT_BLU_ENG_XB2_2,1024",
            "351,CT_RED_ENG_XB2_3,1024",
            "352,CT_GRN_ENG_XB2_3,1024",
            "353,CT_BLU_ENG_XB2_3,1024",
            "354,CT_RED_ENG_XB2_4,1024",
            "355,CT_GRN_ENG_XB2_4,1024",
            "356,CT_BLU_ENG_XB2_4,1024",
            "357,CT_RED_ENG_XB3_1,1024",
            "358,CT_GRN_ENG_XB3_1,1024",
            "359,CT_BLU_ENG_XB3_1,1024",
            "360,CT_RED_ENG_XB3_2,1024",
            "361,CT_GRN_ENG_XB3_2,1024",
            "362,CT_BLU_ENG_XB3_2,1024",
            "363,CT_RED_ENG_XB3_3,1024",
            "364,CT_GRN_ENG_XB3_3,1024",
            "365,CT_BLU_ENG_XB3_3,1024",
            "366,CT_RED_ENG_XB3_4,1024",
            "367,CT_GRN_ENG_XB3_4,1024",
            "368,CT_BLU_ENG_XB3_4,1024",
            "369,CT_RED_ENG_XB4_1,1024",
            "370,CT_GRN_ENG_XB4_1,1024",
            "371,CT_BLU_ENG_XB4_1,1024",
            "372,CT_RED_ENG_XB4_2,1024",
            "373,CT_GRN_ENG_XB4_2,1024",
            "374,CT_BLU_ENG_XB4_2,1024",
            "375,CT_RED_ENG_XB4_3,1024",
            "376,CT_GRN_ENG_XB4_3,1024",
            "377,CT_BLU_ENG_XB4_3,1024",
            "378,CT_RED_ENG_XB4_4,1024",
            "379,CT_GRN_ENG_XB4_4,1024",
            "380,CT_BLU_ENG_XB4_4,1024",
            "381,WT_GMA1_RED_ENG_XB1_1,512",
            "382,WT_GMA2_RED_ENG_XB1_1,1024",
            "383,WT_GMA3_RED_ENG_XB1_1,2048",
            "384,WT_GMA4_RED_ENG_XB1_1,4080",
            "385,WT_GMA1_GRN_ENG_XB1_1,512",
            "386,WT_GMA2_GRN_ENG_XB1_1,1024",
            "387,WT_GMA3_GRN_ENG_XB1_1,2048",
            "388,WT_GMA4_GRN_ENG_XB1_1,4080",
            "389,WT_GMA1_BLU_ENG_XB1_1,512",
            "390,WT_GMA2_BLU_ENG_XB1_1,1024",
            "391,WT_GMA3_BLU_ENG_XB1_1,2048",
            "392,WT_GMA4_BLU_ENG_XB1_1,4080",
            "393,WT_GMA1_RED_ENG_XB1_2,512",
            "394,WT_GMA2_RED_ENG_XB1_2,1024",
            "395,WT_GMA3_RED_ENG_XB1_2,2048",
            "396,WT_GMA4_RED_ENG_XB1_2,4080",
            "397,WT_GMA1_GRN_ENG_XB1_2,512",
            "398,WT_GMA2_GRN_ENG_XB1_2,1024",
            "399,WT_GMA3_GRN_ENG_XB1_2,2048",
            "400,WT_GMA4_GRN_ENG_XB1_2,4080",
            "401,WT_GMA1_BLU_ENG_XB1_2,512",
            "402,WT_GMA2_BLU_ENG_XB1_2,1024",
            "403,WT_GMA3_BLU_ENG_XB1_2,2048",
            "404,WT_GMA4_BLU_ENG_XB1_2,4080",
            "405,WT_GMA1_RED_ENG_XB1_3,512",
            "406,WT_GMA2_RED_ENG_XB1_3,1024",
            "407,WT_GMA3_RED_ENG_XB1_3,2048",
            "408,WT_GMA4_RED_ENG_XB1_3,4080",
            "409,WT_GMA1_GRN_ENG_XB1_3,512",
            "410,WT_GMA2_GRN_ENG_XB1_3,1024",
            "411,WT_GMA3_GRN_ENG_XB1_3,2048",
            "412,WT_GMA4_GRN_ENG_XB1_3,4080",
            "413,WT_GMA1_BLU_ENG_XB1_3,512",
            "414,WT_GMA2_BLU_ENG_XB1_3,1024",
            "415,WT_GMA3_BLU_ENG_XB1_3,2048",
            "416,WT_GMA4_BLU_ENG_XB1_3,4080",
            "417,WT_GMA1_RED_ENG_XB1_4,512",
            "418,WT_GMA2_RED_ENG_XB1_4,1024",
            "419,WT_GMA3_RED_ENG_XB1_4,2048",
            "420,WT_GMA4_RED_ENG_XB1_4,4080",
            "421,WT_GMA1_GRN_ENG_XB1_4,512",
            "422,WT_GMA2_GRN_ENG_XB1_4,1024",
            "423,WT_GMA3_GRN_ENG_XB1_4,2048",
            "424,WT_GMA4_GRN_ENG_XB1_4,4080",
            "425,WT_GMA1_BLU_ENG_XB1_4,512",
            "426,WT_GMA2_BLU_ENG_XB1_4,1024",
            "427,WT_GMA3_BLU_ENG_XB1_4,2048",
            "428,WT_GMA4_BLU_ENG_XB1_4,4080",
            "429,WT_GMA1_RED_ENG_XB2_1,512",
            "430,WT_GMA2_RED_ENG_XB2_1,1024",
            "431,WT_GMA3_RED_ENG_XB2_1,2048",
            "432,WT_GMA4_RED_ENG_XB2_1,4080",
            "433,WT_GMA1_GRN_ENG_XB2_1,512",
            "434,WT_GMA2_GRN_ENG_XB2_1,1024",
            "435,WT_GMA3_GRN_ENG_XB2_1,2048",
            "436,WT_GMA4_GRN_ENG_XB2_1,4080",
            "437,WT_GMA1_BLU_ENG_XB2_1,512",
            "438,WT_GMA2_BLU_ENG_XB2_1,1024",
            "439,WT_GMA3_BLU_ENG_XB2_1,2048",
            "440,WT_GMA4_BLU_ENG_XB2_1,4080",
            "441,WT_GMA1_RED_ENG_XB2_2,512",
            "442,WT_GMA2_RED_ENG_XB2_2,1024",
            "443,WT_GMA3_RED_ENG_XB2_2,2048",
            "444,WT_GMA4_RED_ENG_XB2_2,4080",
            "445,WT_GMA1_GRN_ENG_XB2_2,512",
            "446,WT_GMA2_GRN_ENG_XB2_2,1024",
            "447,WT_GMA3_GRN_ENG_XB2_2,2048",
            "448,WT_GMA4_GRN_ENG_XB2_2,4080",
            "449,WT_GMA1_BLU_ENG_XB2_2,512",
            "450,WT_GMA2_BLU_ENG_XB2_2,1024",
            "451,WT_GMA3_BLU_ENG_XB2_2,2048",
            "452,WT_GMA4_BLU_ENG_XB2_2,4080",
            "453,WT_GMA1_RED_ENG_XB2_3,512",
            "454,WT_GMA2_RED_ENG_XB2_3,1024",
            "455,WT_GMA3_RED_ENG_XB2_3,2048",
            "456,WT_GMA4_RED_ENG_XB2_3,4080",
            "457,WT_GMA1_GRN_ENG_XB2_3,512",
            "458,WT_GMA2_GRN_ENG_XB2_3,1024",
            "459,WT_GMA3_GRN_ENG_XB2_3,2048",
            "460,WT_GMA4_GRN_ENG_XB2_3,4080",
            "461,WT_GMA1_BLU_ENG_XB2_3,512",
            "462,WT_GMA2_BLU_ENG_XB2_3,1024",
            "463,WT_GMA3_BLU_ENG_XB2_3,2048",
            "464,WT_GMA4_BLU_ENG_XB2_3,4080",
            "465,WT_GMA1_RED_ENG_XB2_4,512",
            "466,WT_GMA2_RED_ENG_XB2_4,1024",
            "467,WT_GMA3_RED_ENG_XB2_4,2048",
            "468,WT_GMA4_RED_ENG_XB2_4,4080",
            "469,WT_GMA1_GRN_ENG_XB2_4,512",
            "470,WT_GMA2_GRN_ENG_XB2_4,1024",
            "471,WT_GMA3_GRN_ENG_XB2_4,2048",
            "472,WT_GMA4_GRN_ENG_XB2_4,4080",
            "473,WT_GMA1_BLU_ENG_XB2_4,512",
            "474,WT_GMA2_BLU_ENG_XB2_4,1024",
            "475,WT_GMA3_BLU_ENG_XB2_4,2048",
            "476,WT_GMA4_BLU_ENG_XB2_4,4080",
            "477,WT_GMA1_RED_ENG_XB3_1,512",
            "478,WT_GMA2_RED_ENG_XB3_1,1024",
            "479,WT_GMA3_RED_ENG_XB3_1,2048",
            "480,WT_GMA4_RED_ENG_XB3_1,4080",
            "481,WT_GMA1_GRN_ENG_XB3_1,512",
            "482,WT_GMA2_GRN_ENG_XB3_1,1024",
            "483,WT_GMA3_GRN_ENG_XB3_1,2048",
            "484,WT_GMA4_GRN_ENG_XB3_1,4080",
            "485,WT_GMA1_BLU_ENG_XB3_1,512",
            "486,WT_GMA2_BLU_ENG_XB3_1,1024",
            "487,WT_GMA3_BLU_ENG_XB3_1,2048",
            "488,WT_GMA4_BLU_ENG_XB3_1,4080",
            "489,WT_GMA1_RED_ENG_XB3_2,512",
            "490,WT_GMA2_RED_ENG_XB3_2,1024",
            "491,WT_GMA3_RED_ENG_XB3_2,2048",
            "492,WT_GMA4_RED_ENG_XB3_2,4080",
            "493,WT_GMA1_GRN_ENG_XB3_2,512",
            "494,WT_GMA2_GRN_ENG_XB3_2,1024",
            "495,WT_GMA3_GRN_ENG_XB3_2,2048",
            "496,WT_GMA4_GRN_ENG_XB3_2,4080",
            "497,WT_GMA1_BLU_ENG_XB3_2,512",
            "498,WT_GMA2_BLU_ENG_XB3_2,1024",
            "499,WT_GMA3_BLU_ENG_XB3_2,2048",
            "500,WT_GMA4_BLU_ENG_XB3_2,4080",
            "501,WT_GMA1_RED_ENG_XB3_3,512",
            "502,WT_GMA2_RED_ENG_XB3_3,1024",
            "503,WT_GMA3_RED_ENG_XB3_3,2048",
            "504,WT_GMA4_RED_ENG_XB3_3,4080",
            "505,WT_GMA1_GRN_ENG_XB3_3,512",
            "506,WT_GMA2_GRN_ENG_XB3_3,1024",
            "507,WT_GMA3_GRN_ENG_XB3_3,2048",
            "508,WT_GMA4_GRN_ENG_XB3_3,4080",
            "509,WT_GMA1_BLU_ENG_XB3_3,512",
            "510,WT_GMA2_BLU_ENG_XB3_3,1024",
            "511,WT_GMA3_BLU_ENG_XB3_3,2048",
            "512,WT_GMA4_BLU_ENG_XB3_3,4080",
            "513,WT_GMA1_RED_ENG_XB3_4,512",
            "514,WT_GMA2_RED_ENG_XB3_4,1024",
            "515,WT_GMA3_RED_ENG_XB3_4,2048",
            "516,WT_GMA4_RED_ENG_XB3_4,4080",
            "517,WT_GMA1_GRN_ENG_XB3_4,512",
            "518,WT_GMA2_GRN_ENG_XB3_4,1024",
            "519,WT_GMA3_GRN_ENG_XB3_4,2048",
            "520,WT_GMA4_GRN_ENG_XB3_4,4080",
            "521,WT_GMA1_BLU_ENG_XB3_4,512",
            "522,WT_GMA2_BLU_ENG_XB3_4,1024",
            "523,WT_GMA3_BLU_ENG_XB3_4,2048",
            "524,WT_GMA4_BLU_ENG_XB3_4,4080",
            "525,WT_GMA1_RED_ENG_XB4_1,512",
            "526,WT_GMA2_RED_ENG_XB4_1,1024",
            "527,WT_GMA3_RED_ENG_XB4_1,2048",
            "528,WT_GMA4_RED_ENG_XB4_1,4080",
            "529,WT_GMA1_GRN_ENG_XB4_1,512",
            "530,WT_GMA2_GRN_ENG_XB4_1,1024",
            "531,WT_GMA3_GRN_ENG_XB4_1,2048",
            "532,WT_GMA4_GRN_ENG_XB4_1,4080",
            "533,WT_GMA1_BLU_ENG_XB4_1,512",
            "534,WT_GMA2_BLU_ENG_XB4_1,1024",
            "535,WT_GMA3_BLU_ENG_XB4_1,2048",
            "536,WT_GMA4_BLU_ENG_XB4_1,4080",
            "537,WT_GMA1_RED_ENG_XB4_2,512",
            "538,WT_GMA2_RED_ENG_XB4_2,1024",
            "539,WT_GMA3_RED_ENG_XB4_2,2048",
            "540,WT_GMA4_RED_ENG_XB4_2,4080",
            "541,WT_GMA1_GRN_ENG_XB4_2,512",
            "542,WT_GMA2_GRN_ENG_XB4_2,1024",
            "543,WT_GMA3_GRN_ENG_XB4_2,2048",
            "544,WT_GMA4_GRN_ENG_XB4_2,4080",
            "545,WT_GMA1_BLU_ENG_XB4_2,512",
            "546,WT_GMA2_BLU_ENG_XB4_2,1024",
            "547,WT_GMA3_BLU_ENG_XB4_2,2048",
            "548,WT_GMA4_BLU_ENG_XB4_2,4080",
            "549,WT_GMA1_RED_ENG_XB4_3,512",
            "550,WT_GMA2_RED_ENG_XB4_3,1024",
            "551,WT_GMA3_RED_ENG_XB4_3,2048",
            "552,WT_GMA4_RED_ENG_XB4_3,4080",
            "553,WT_GMA1_GRN_ENG_XB4_3,512",
            "554,WT_GMA2_GRN_ENG_XB4_3,1024",
            "555,WT_GMA3_GRN_ENG_XB4_3,2048",
            "556,WT_GMA4_GRN_ENG_XB4_3,4080",
            "557,WT_GMA1_BLU_ENG_XB4_3,512",
            "558,WT_GMA2_BLU_ENG_XB4_3,1024",
            "559,WT_GMA3_BLU_ENG_XB4_3,2048",
            "560,WT_GMA4_BLU_ENG_XB4_3,4080",
            "561,WT_GMA1_RED_ENG_XB4_4,512",
            "562,WT_GMA2_RED_ENG_XB4_4,1024",
            "563,WT_GMA3_RED_ENG_XB4_4,2048",
            "564,WT_GMA4_RED_ENG_XB4_4,4080",
            "565,WT_GMA1_GRN_ENG_XB4_4,512",
            "566,WT_GMA2_GRN_ENG_XB4_4,1024",
            "567,WT_GMA3_GRN_ENG_XB4_4,2048",
            "568,WT_GMA4_GRN_ENG_XB4_4,4080",
            "569,WT_GMA1_BLU_ENG_XB4_4,512",
            "570,WT_GMA2_BLU_ENG_XB4_4,1024",
            "571,WT_GMA3_BLU_ENG_XB4_4,2048",
            "572,WT_GMA4_BLU_ENG_XB4_4,4080"

        };

        string[,] svuiregadrTB = null;

        public byte dualduty = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svact"></param>
        /// <param name="svip"></param>
        /// <param name="svsen"></param>
        /// <param name="svpo"></param>
        /// <param name="svca"></param>
        public void cENGGMAONWRITEp(byte svact, byte svip, byte svsen, byte svpo, byte svca,Dictionary<int,List<int>>FPGA_B, Dictionary<int, List<int>> FPGA_A)
        {
            // 初始化 Reg
            Array.Resize(ref uiregadr_default, uiregadr_default_p.Length);
            uiregadr_default = uiregadr_default_p;

            markreset(999, false);
            flgDelFB = true;
            //List<string> svuiregadr = new List<string>(new string[GAMMA_SIZE / 4]);
            
            string[] sRegDec;
            string[] sDataDec;
            string[] svuiuser = new string[40];

            string svdeviceID = deviceID;

            isReadBack = true;

            //if (Form1.lstm.Items.Count == 0) Form1.lstm.Items.Add(mvars.deviceID.Substring(0, 2) + "01");
            svuiregadrTB = new string[lstm.Items.Count, GAMMA_SIZE / 4];
            #region Primary
            if (svact == 0)
            {
                #region ENGGMA,ON
                lstget1.Items.Clear();
                lstsvuiregadr.Items.Clear();
                svuiregadrTB = new string[lstm.Items.Count, GAMMA_SIZE / 4];
                for (svpo = 0; svpo < lstm.Items.Count; svpo++)
                {
                    deviceID = deviceID.Substring(0, 2) + lstm.Items[svpo].ToString().Substring(2, 2);
                    FPGAsel = svca;
                    int svms = 91;                              /// Primary
                    int svme = uiregadr_default.Length;   /// Primary
                    if (dualduty == 0)
                    {
                        for (int svi = 0; svi < uiregadr_default.Length; svi++)
                        {
                            if (uiregadr_default[svi].IndexOf("WT_GMA", 0) != -1) { svms = svi; break; }
                        }
                        if (svsen == 160)
                        {
                            //單屏
                            for (int svi = svms; svi < uiregadr_default.Length; svi++)
                            {
                                if (uiregadr_default[svi].IndexOf("IDLE", 0) != -1) { svme = svi; break; }
                            }
                        }
                        else
                        {
                            //條屏
                        }
                    }
                    lblCmd = "UIREGRAD_READ";
                    mUIREGARDRm(0, svme);
                    if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-1"; }
                    else
                    {
                        for (int i = 0; i < svme; i++)
                        {
                            svuiregadr[i] = (ReadDataBuffer[6 + i * 2 + 1] * 256 + ReadDataBuffer[7 + i * 2 + 1]).ToString();
                            svuiregadr[i + 1024] = (ReadDataBuffer[6 + i * 2 + svme * 2 + 1] * 256 + ReadDataBuffer[7 + i * 2 + svme * 2 + 1]).ToString();
                            svuiregadr[i] = (ReadDataBuffer[6 + i * 2 + 1] * 256 + ReadDataBuffer[7 + i * 2 + 1]).ToString();
                            svuiregadr[i + 1024] = (ReadDataBuffer[6 + i * 2 + svme * 2 + 1] * 256 + ReadDataBuffer[7 + i * 2 + svme * 2 + 1]).ToString();
                            svuiregadrTB[svpo, i] = (ReadDataBuffer[6 + i * 2 + 1] * 256 + ReadDataBuffer[7 + i * 2 + 1]).ToString();
                            svuiregadrTB[svpo, i + 1024] = (ReadDataBuffer[6 + i * 2 + svme * 2 + 1] * 256 + ReadDataBuffer[7 + i * 2 + svme * 2 + 1]).ToString();
                        }
                        lstsvuiregadr.Items.Clear();
                        for (int i = 0; i < svuiregadr.Count; i++)
                        {
                            if (svuiregadr[i] != null) lstsvuiregadr.Items.Add(svuiregadr[i]);
                            else lstsvuiregadr.Items.Add(" ");
                        }



                        lstget1.Items.Add(string.Format("No.{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                                                (svpo + 1).ToString(),
                                                lstsvuiregadr.Items[91].ToString(),
                                                lstsvuiregadr.Items[92].ToString(),
                                                lstsvuiregadr.Items[93].ToString(),
                                                lstsvuiregadr.Items[94].ToString(),
                                                lstsvuiregadr.Items[95].ToString(),
                                                lstsvuiregadr.Items[96].ToString(),
                                                lstsvuiregadr.Items[97].ToString(),
                                                lstsvuiregadr.Items[98].ToString(),
                                                lstsvuiregadr.Items[99].ToString(),
                                                lstsvuiregadr.Items[100].ToString(),
                                                lstsvuiregadr.Items[101].ToString(),
                                                lstsvuiregadr.Items[102].ToString()));

                        #region 還原 Drop (0523 disabled)
                        //if (svca == 2)
                        //{
                        //    for (byte svn = 0; svn < 2; svn++)
                        //    {
                        //        int svj = (int)(mvars.GAMMA_SIZE / 8 * svn);
                        //        for (int i = 0; i < mvars.uiregadr_default.Length; i++)
                        //            Form1.svuiregadr[i + svj] = mvars.uiregadr_default[i].Split(',')[2];
                        //        Form1.svuiregadr[5 + svj] = "1";
                        //        sRegDec = new string[svme];   //addr
                        //        sDataDec = new string[svme];  //data
                        //        for (int i = 0; i < svme; i++)
                        //        {
                        //            sRegDec[i] = i.ToString();
                        //            sDataDec[i] = Form1.svuiregadr[i + svj];
                        //        }
                        //        mvars.lblCmd = "FPGA_REG_W" + svn;
                        //        mp.mpFPGAUIREGWarr(sRegDec, sDataDec);
                        //    }
                        //}
                        //else
                        //{
                        //    //單屏右側(svca=0)左側(svca=1)還原預設值
                        //    int svj = (int)(mvars.GAMMA_SIZE / 8 * svca);
                        //    for (int i = 0; i < mvars.uiregadr_default.Length; i++)
                        //        Form1.svuiregadr[i + svj] = mvars.uiregadr_default[i].Split(',')[2];
                        //    Form1.svuiregadr[5 + svj] = "1";
                        //    sRegDec = new string[svme];   //addr
                        //    sDataDec = new string[svme];  //data
                        //    for (int i = 0; i < svme; i++)
                        //    {
                        //        sRegDec[i] = i.ToString();
                        //        sDataDec[i] = Form1.svuiregadr[i + svj];
                        //    }
                        //    mvars.lblCmd = "FPGA_REG_W" + svca;
                        //    mp.mpFPGAUIREGWarr(sRegDec, sDataDec);
                        //}
                        #endregion 還原 Drop

                        #region ENG_GMA_EN OFF
                        //Read mode
                        pvindex = 32;
                        lblCmd = "FPGA_SPI_W";
                        mhFPGASPIWRITE(FPGAsel, 1);
                        if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-" + pvindex; }
                        //Addr
                        pvindex = 33;
                        lblCmd = "FPGA_SPI_W";
                        mhFPGASPIWRITE(FPGAsel, 5);
                        if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-" + pvindex; }
                        //WData
                        pvindex = 34;
                        lblCmd = "FPGA_SPI_W";
                        mhFPGASPIWRITE(FPGAsel, 0);
                        if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-" + pvindex; }
                        //Write mode
                        pvindex = 32;
                        lblCmd = "FPGA_SPI_W";
                        mhFPGASPIWRITE(FPGAsel, 0);
                        if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-" + pvindex; }

                        //Read mode
                        pvindex = 32;
                        lblCmd = "FPGA_SPI_W";
                        mhFPGASPIWRITE(FPGAsel, 1);
                        if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-" + pvindex; }
                        #endregion ENG_GMA_EN OFF
                    }
                    for (int svi = 0; svi < svuiregadr.Count; svi++) svuiregadr[svi] = svuiregadr[svi];
                }
                #endregion ENGGMA,ON
            }
            else if (svact == 1)
            {
                #region ENGGMA
                deviceID = deviceID.Substring(0, 2) + DecToHex(Convert.ToInt16(svpo), 2);
                FPGAsel = svca;
                int svms = 91;                              /// Primary
                int svme = uiregadr_default.Length;   /// Primary
                if (dualduty == 0)
                {
                    for (int svi = 0; svi < uiregadr_default.Length; svi++)
                    {
                        if (uiregadr_default[svi].IndexOf("WT_GMA", 0) != -1) { svms = svi; break; }
                    }
                    //單屏
                    for (int svi = svms; svi < uiregadr_default.Length; svi++)
                    {
                        if (uiregadr_default[svi].IndexOf("IDLE", 0) != -1) { svme = svi; break; }
                    }
                }
                if (svca == 2)
                {
                    for (byte svn = 0; svn < 2; svn++)
                    {
                        int svj = (int)(GAMMA_SIZE / 8 * svn);
                        //在 Form1.tmePull已處理 Form1.svuiregadr 內容
                        svuiregadr[5 + svj] = "1";
                        //svuiregadrTB[svpo-1, 5 + svj] = Form1.svuiregadr[5 + svj];
                        sRegDec = new string[svme];   //addr
                        sDataDec = new string[svme];  //data
                        for (int i = 0; i < svme; i++)
                        {
                            sRegDec[i] = i.ToString();
                            sDataDec[i] = svuiregadr[i + svj];
                            //svuiregadrTB[svpo-1, i + svj] = Form1.svuiregadr[5 + svj];
                        }
                        lblCmd = "FPGA_REG_W" + svn;
                        // 將調整的色差資訊帶入
                        for(int i = 1; i < 5; i++)
                        {
                            for(int j = 0; j < FPGA_B[1].Count; j++)
                            {
                                if(svn == 1)
                                {
                                    sDataDec[(i - 1) * 48 + j + 91] = FPGA_B[i][j].ToString();
                                    FPGAsel = 1;
                                }
                                else
                                {
                                    sDataDec[(i - 1) * 48 + j + 91] = FPGA_A[i][j].ToString();
                                    FPGAsel = 0;
                                }
                            }
                        }
                        mpFPGAUIREGWarr(sRegDec, sDataDec);
                    }
                }
                else
                {
                    //單屏右側(svca=0)左側(svca=1)還原預設值
                    int svj = (int)(GAMMA_SIZE / 8 * svca);
                    //在 Form1.tmePull已處理 Form1.svuiregadr 內容
                    svuiregadr[5 + svj] = "1";
                    //svuiregadrTB[svpo-1, 5 + svj] = Form1.svuiregadr[5 + svj];
                    sRegDec = new string[svme];   //addr
                    sDataDec = new string[svme];  //data
                    for (int i = 0; i < svme; i++)
                    {
                        sRegDec[i] = i.ToString();
                        sDataDec[i] = svuiregadr[i + svj];
                        //svuiregadrTB[svpo-1, i + svj] = Form1.svuiregadr[5 + svj];
                    }
                    lblCmd = "FPGA_REG_W" + svca;
                    mpFPGAUIREGWarr(sRegDec, sDataDec);
                }
                #endregion ENGGMA
            }
            else if (svact == 2)
            {
                #region ENGGMA,WRITE
                deviceID = deviceID.Substring(0, 2) + DecToHex(Convert.ToInt16(svpo), 2);
                FPGAsel = svca;
                byte[] BinArr = new byte[GAMMA_SIZE];
                if (svsen == 160)
                {
                    int svms = 91;      /// Primary
                    int svme = 283;     /// Primary
                    if (dualduty == 1) svme = uiregadr_default.Length;
                    lblCmd = "UIREGRAD_READ";
                    mUIREGARDRm(0, svme);
                    if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-1"; }
                    else
                    {
                        svms = 91;      /// Primary
                        svme = 283;     /// Primary
                        if (dualduty == 1) svme = uiregadr_default.Length;
                        else
                        {
                            for (int i = 0; i < uiregadr_default.Length; i++)
                            {
                                if (uiregadr_default[i].IndexOf("WT_GMA", 0) != -1) { svms = i; break; }
                            }
                            for (int i = svms; i < uiregadr_default.Length; i++)
                            {
                                if (uiregadr_default[i].IndexOf("IDLE", 0) != -1) { svme = i; break; }
                            }
                        }
                        for (int svi = 0; svi < svme; svi++)
                        {
                            svuiregadr[svi] = (ReadDataBuffer[6 + svi * 2 + 1] * 256 + ReadDataBuffer[7 + svi * 2 + 1]).ToString();
                            svuiregadr[svi + 1024] = (ReadDataBuffer[6 + svi * 2 + svme * 2 + 1] * 256 + ReadDataBuffer[7 + svi * 2 + svme * 2 + 1]).ToString();
                            svuiregadrTB[svpo - 1, svi] = svuiregadr[svi];
                            svuiregadrTB[svpo - 1, svi + 1024] = svuiregadr[svi + 1024];
                        }
                        sRegDec = new string[svme];   //addr
                        sDataDec = new string[svme];  //data

                        if (svca == 2)
                        {
                            for (byte svn = 0; svn < svca; svn++)
                            {
                                int svj = (int)(GAMMA_SIZE / 8 * svn);
                                svuiregadr[5 + svj] = "1";
                                for (int i = 0; i < svme; i++)
                                {
                                    //sRegDec[i] = i.ToString();
                                    //sDataDec[i] = Form1.svuiregadr[i + svj];
                                    BinArr[i * 4 + 0 + (GAMMA_SIZE / 2 * svn)] = (Byte)(Convert.ToInt32(i) / 256);
                                    BinArr[i * 4 + 1 + (GAMMA_SIZE / 2 * svn)] = (Byte)(Convert.ToInt32(i) % 256);
                                    BinArr[i * 4 + 2 + (GAMMA_SIZE / 2 * svn)] = (Byte)(Convert.ToInt32(svuiregadr[i + svj]) / 256);
                                    BinArr[i * 4 + 3 + (GAMMA_SIZE / 2 * svn)] = (Byte)(Convert.ToInt32(svuiregadr[i + svj]) % 256);
                                }
                                //mvars.lblCmd = "FPGA_REG_W" + svn;
                                //mp.mpFPGAUIREGWarr(sRegDec, sDataDec);
                            }
                            //Checksum
                            UInt16 checksum = CalChecksum(BinArr, 0, (UInt16)(BinArr.Length - 3));
                            BinArr[BinArr.Length - 2] = (byte)(checksum / 256);
                            BinArr[BinArr.Length - 1] = (byte)(checksum % 256);
                            //Save File
                            string path = "C:\\Users\\" + Environment.UserName + "\\Documents\\BinArr.bin";
                            SaveBinFile(path, BinArr);
                            lblCmd = "MCU_FLASH_W62000";
                            mhMCUFLASHWRITE("62000", ref BinArr);
                            if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-1"; }
                        }
                        else
                        {
                            //單屏右側(svca=0)左側(svca=1)還原預設值
                            int svj = (int)(GAMMA_SIZE / 8 * svca);
                            svuiregadr[5 + svj] = "1";
                            if (svca == 0)
                            {
                                for (int i = 0; i < svme; i++)
                                {
                                    BinArr[i * 4 + 0] = (Byte)(Convert.ToInt32(i) / 256);
                                    BinArr[i * 4 + 1] = (Byte)(Convert.ToInt32(i) % 256);
                                    BinArr[i * 4 + 2] = (Byte)(Convert.ToInt32(svuiregadr[i]) / 256);
                                    BinArr[i * 4 + 3] = (Byte)(Convert.ToInt32(svuiregadr[i]) % 256);
                                }
                            }
                            else if (svca == 1)
                            {
                                for (int i = 0; i < svme; i++)
                                {
                                    BinArr[i * 4 + 0 + (GAMMA_SIZE / 2 * svca)] = (Byte)(Convert.ToInt32(i) / 256);
                                    BinArr[i * 4 + 1 + (GAMMA_SIZE / 2 * svca)] = (Byte)(Convert.ToInt32(i) % 256);
                                    BinArr[i * 4 + 2 + (GAMMA_SIZE / 2 * svca)] = (Byte)(Convert.ToInt32(svuiregadr[i + svj]) / 256);
                                    BinArr[i * 4 + 3 + (GAMMA_SIZE / 2 * svca)] = (Byte)(Convert.ToInt32(svuiregadr[i + svj]) % 256);
                                }
                            }
                            //Checksum
                            UInt16 checksum = CalChecksum(BinArr, 0, (UInt16)(BinArr.Length - 3));
                            BinArr[BinArr.Length - 2] = (byte)(checksum / 256);
                            BinArr[BinArr.Length - 1] = (byte)(checksum % 256);

                            //Save File
                            string path = "C:\\Users\\" + Environment.UserName + "\\Documents\\BinArr.bin";
                            SaveBinFile(path, BinArr);
                            lblCmd = "MCU_FLASH_W62000";
                            mhMCUFLASHWRITE("62000", ref BinArr);
                            if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-1"; }
                        }
                        #region 回讀 0x64000
                        lblCmd = "MCU_FLASH_R64000";
                        mhMCUFLASHREAD("00064000", 8192);
                        if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1)
                        {
                            errCode = "-3";

                        }
                        else
                        {
                            if (strR64K.Length > 1)
                            {
                                //List<string> lst = new List<string>(new string[mvars.strR64K.Split('~').Length]);
                                List<string> lst = new List<string>();
                                lst.AddRange(strR64K.Split('~'));
                                for (int svi = 0; svi < svuiuser.Length; svi++)
                                {
                                    svuiuser[svi] = lst[svi].Split(',')[1];
                                }
                                byte svFMgmaENreg = 5;
                                svuiuser[svFMgmaENreg] = "1";

                                for (UInt16 i = 0; i < svuiuser.Length; i++)
                                {
                                    BinArr[i * 4 + 0] = (Byte)(Convert.ToInt32(i) / 256);
                                    BinArr[i * 4 + 1] = (Byte)(Convert.ToInt32(i) % 256);
                                    BinArr[i * 4 + 2] = (Byte)(Convert.ToInt32(svuiuser[i]) / 256);
                                    BinArr[i * 4 + 3] = (Byte)(Convert.ToInt32(svuiuser[i]) % 256);
                                }

                                //Checksum
                                UInt16 checksum = CalChecksum(BinArr, 0, (UInt16)(BinArr.Length - 3));
                                BinArr[BinArr.Length - 2] = (byte)(checksum / 256);
                                BinArr[BinArr.Length - 1] = (byte)(checksum % 256);

                                //Save File
                                //path = "C:\\Users\\" + Environment.UserName + "\\Documents\\BinArr.bin";
                                //SaveBinFile(path, BinArr);

                                lblCmd = "MCU_FLASH_W64000";
                                mhMCUFLASHWRITE("64000", ref BinArr);
                                if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-1"; }
                            }
                            //else lbl_mcuR64000click.Text = "no record";
                            //lbl_mcuR64000click.Text = "< ";
                            //Form1.tslblStatus.Text = "0x64000 rd MCU Flash items：" + (mvars.lstmcuR64000.Items.Count).ToString();
                        }
                        #endregion 回讀 0x64000

                    }
                }
                #endregion ENGGMA,WRITE
            }
            else if (svact == 3)
            {
                #region ENGGMA OFF
                deviceID = deviceID.Substring(0, 2) + DecToHex(Convert.ToInt16(svpo), 2);
                FPGAsel = svca;
                int svms = 91;                              /// Primary
                int svme = uiregadr_default.Length;   /// Primary
                if (dualduty == 0)
                {
                    for (int svi = 0; svi < uiregadr_default.Length; svi++)
                    {
                        if (uiregadr_default[svi].IndexOf("WT_GMA", 0) != -1) { svms = svi; break; }
                    }
                    //單屏
                    for (int svi = svms; svi < uiregadr_default.Length; svi++)
                    {
                        if (uiregadr_default[svi].IndexOf("IDLE", 0) != -1) { svme = svi; break; }
                    }
                }
                for (svpo = 0; svpo < lstm.Items.Count; svpo++)
                {
                    deviceID = deviceID.Substring(0, 2) + lstm.Items[svpo].ToString().Substring(2, 2);
                    FPGAsel = svca;

                    for (int i = 0; i < svme; i++)
                    {
                        svuiregadr[i] = svuiregadrTB[svpo, i];
                        svuiregadr[i + 1024] = svuiregadrTB[svpo, i + 1024];
                    }
                    //Form1.lstsvuiregadr.Items.Clear();
                    //for (int i = 0; i < Form1.svuiregadr.Count; i++)
                    //{
                    //    if (Form1.svuiregadr[i] != null) Form1.lstsvuiregadr.Items.Add(Form1.svuiregadr[i]);
                    //    else Form1.lstsvuiregadr.Items.Add(" ");
                    //}
                    #region 取自 ENG_GMA
                    if (svca == 2)
                    {
                        for (byte svn = 0; svn < 2; svn++)
                        {
                            int svj = (int)(GAMMA_SIZE / 8 * svn);
                            svuiregadr[5 + svj] = "1";
                            sRegDec = new string[svme];   //addr
                            sDataDec = new string[svme];  //data
                            for (int i = 0; i < svme; i++)
                            {
                                sRegDec[i] = i.ToString();
                                sDataDec[i] = svuiregadr[i + svj];
                            }
                            lblCmd = "FPGA_REG_W" + svn;
                            mpFPGAUIREGWarr(sRegDec, sDataDec);
                        }
                    }
                    else
                    {
                        //單屏右側(svca=0)左側(svca=1)還原預設值
                        int svj = (int)(GAMMA_SIZE / 8 * svca);
                        svuiregadr[5 + svj] = "1";
                        sRegDec = new string[svme];   //addr
                        sDataDec = new string[svme];  //data
                        for (int i = 0; i < svme; i++)
                        {
                            sRegDec[i] = i.ToString();
                            sDataDec[i] = svuiregadr[i + svj];
                        }
                        lblCmd = "FPGA_REG_W" + svca;
                        mpFPGAUIREGWarr(sRegDec, sDataDec);
                    }
                    #endregion 取自ENG_GMA
                }
                #endregion ENGGMA OFF
            }
            else if (svip == 160 && svsen == 160 && svpo == 160)
            {
                #region 廣播+畫面左側/畫面右側
                deviceID = "05A0";
                FPGAsel = svca;
                #endregion 廣播
            }
            else
            {
                #region 指定單屏+單屏/左/右
                deviceID = deviceID.Substring(0, 2) + DecToHex(Convert.ToInt16(svpo), 2);
                FPGAsel = svca;
                #endregion
            }

            #endregion Primary

            if (svnova == false && sp1.IsOpen) { CommClose(); }

            deviceID = svdeviceID;
            flgDelFB = false;
            lCounts = lCount + 1;
            lblCmd = "EndcCMD"; lCmd[lCount] = lblCmd + ",";
            flgSend = true; flgReceived = false;
            flgReceived = true;
        }

        public void cPGRGB10BITp(byte svtype, byte svip, byte svsen, byte svpo, ushort svca, string svr, string svg, string svb)   //1010
        {
            markreset(999, false);

            flgDelFB = true;

            #region USB
            if (svip == 0 && svsen == 0 && svpo == 0) deviceID = "05A0";
            else
            {
                if (svpo > 0) deviceID = "05" + DecToHex(Convert.ToInt16(svpo), 2);
            }
            if (svca <= 1) FPGAsel = (byte)svca;
            else if (svca > 1) FPGAsel = 2;
            isReadBack = false;
            if (svtype == 0)
            {
                FPGAsel = 2;
                int[] svreg = { 1, 48, 49, 50, 51, 52 };         //PC mode
                int[] svdata = { 1, 1023, 1023, 1023, 0, 360 };
                lblCmd = "FPGA_SPI_W";
                if (deviceID.Substring(0, 2) == "05") mhFPGASPIWRITE(FPGAsel, svreg, svdata);
                //else mp.mhFPGASPIWRITE(Convert.ToInt32(lblFPGAtxt[Svi].Text));
                if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-1"; }
            }
            else if (svtype == 255)
            {
                FPGAsel = 2;
                int[] svreg = { 1, 48, 49, 50, 51, 52 };         //(1:PG mode)+(48:R)(49:G)(50:B)+(51_PT_Bank)+(52:PG_auto)
                int[] svdata = { 96, 0, 0, 512, 3, 360 };
                lblCmd = "FPGA_SPI_W";
                if (deviceID.Substring(0, 2) == "05") mhFPGASPIWRITE(FPGAsel, svreg, svdata);
                //else mp.mhFPGASPIWRITE(Convert.ToInt32(lblFPGAtxt[Svi].Text));
                if (lGet[lCount - 1].IndexOf("ERROR", 0) > -1) { errCode = "-1"; }
            }
            else if (svtype == 2)
            {

                int[] svreg = { 1, 48, 49, 50, 51, 52 };         //(1:PG mode)+(48:R)(49:G)(50:B)+(51_PT_Bank)+(52:PG_auto)
                int[] svdata = { 96, int.Parse(svr), int.Parse(svg), int.Parse(svb), 3, 360 };
                lblCmd = "FPGA_SPI_W";
                // Fred 魔改
                FPGAsel = 2;
                //int[] svreg = { 1, 51, 52, 53, 54, 55, 56, 57, 58, 59 };
                //svdata = new int[] { 128, 12, 360, int.Parse(svr), int.Parse(svg), int.Parse(svb), 0, 0, 120, 540 };
                mhFPGASPIWRITE(FPGAsel, svreg, svdata);
            }
            else if (svtype == 3)
            {
                //int[] svreg = { 1, 51, 52, 53, 54, 55, 56, 57, 58, 59 };    //(1:PG mode)+(51_PT_Bank)+(52:PG_auto)+(53:R1)(54:G1)(55:B1)+(56:X)(57:Y)(58:W)(59:H)
                int[] svreg = { 1, 51, 52, 48, 49, 50, 56, 57, 58, 59}; // 52 為 Delay time
                int svx = 0;
                int svy = 191;
                int svw = 120;
                int svh = 135;
                if (svsen == 1) { svx = 0; svy = 0; FPGAsel = 1; }
                else if (svsen == 2) { svx = 120; svy = 0; FPGAsel = 1; }
                else if (svsen == 3) { svx = 240; svy = 0; FPGAsel = 1; }
                else if (svsen == 4) { svx = 360; svy = 0; FPGAsel = 1; }
                else if (svsen == 5) { svx = 0; svy = 0; FPGAsel = 0; }
                else if (svsen == 6) { svx = 120; svy = 0; FPGAsel = 0; }
                else if (svsen == 7) { svx = 240; svy = 0; FPGAsel = 0; }
                else if (svsen == 8) { svx = 360; svy = 0; FPGAsel = 0; }

                else if (svsen == 9) { svx = 0; svy = 135; FPGAsel = 1; }
                else if (svsen == 10) { svx = 120; svy = 135; FPGAsel = 1; }
                else if (svsen == 11) { svx = 240; svy = 135; FPGAsel = 1; }
                else if (svsen == 12) { svx = 360; svy = 135; FPGAsel = 1; }
                else if (svsen == 13) { svx = 0; svy = 135; FPGAsel = 0; }
                else if (svsen == 14) { svx = 120; svy = 135; FPGAsel = 0; }
                else if (svsen == 15) { svx = 240; svy = 135; FPGAsel = 0; }
                else if (svsen == 16) { svx = 360; svy = 135; FPGAsel = 0; }

                else if (svsen == 17) { svx = 0; svy = 270; FPGAsel = 1; }
                else if (svsen == 18) { svx = 120; svy = 270; FPGAsel = 1; }
                else if (svsen == 19) { svx = 240; svy = 270; FPGAsel = 1; }
                else if (svsen == 20) { svx = 360; svy = 270; FPGAsel = 1; }
                else if (svsen == 21) { svx = 0; svy = 270; FPGAsel = 0; }
                else if (svsen == 22) { svx = 120; svy = 270; FPGAsel = 0; }
                else if (svsen == 23) { svx = 240; svy = 270; FPGAsel = 0; }
                else if (svsen == 24) { svx = 360; svy = 270; FPGAsel = 0; }

                else if (svsen == 25) { svx = 0; svy = 405; FPGAsel = 1; }
                else if (svsen == 26) { svx = 120; svy = 405; FPGAsel = 1; }
                else if (svsen == 27) { svx = 240; svy = 405; FPGAsel = 1; }
                else if (svsen == 28) { svx = 360; svy = 405; FPGAsel = 1; }
                else if (svsen == 29) { svx = 0; svy = 405; FPGAsel = 0; }
                else if (svsen == 30) { svx = 120; svy = 405; FPGAsel = 0; }
                else if (svsen == 31) { svx = 240; svy = 405; FPGAsel = 0; }
                else if (svsen == 32) { svx = 360; svy = 405; FPGAsel = 0; }

                int[] svdata = new int[] { 96, 12, 360, int.Parse(svr), int.Parse(svg), int.Parse(svb), svx, svy, svw, svh };
                lblCmd = "FPGA_SPI_W";
                mhFPGASPIWRITE(FPGAsel, svreg, svdata);
                #endregion
            }
        }



        public void mUIREGARDRm(int svregSt, int svregEd)                //0x12 multi register 
        {
            #region 2023版公用程序 (無 Nova 參數)
            //byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 8193);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion

            RS485_WriteDataBuffer[2 + 1] = 0x12;                      //Cmd
            RS485_WriteDataBuffer[3 + 1] = 0x00;                      //Size
            RS485_WriteDataBuffer[4 + 1] = 0x0D;                      //Size
            RS485_WriteDataBuffer[5 + 1] = (byte)(svregSt / 256);     //Reg Address
            RS485_WriteDataBuffer[6 + 1] = (byte)(svregSt % 256);     //Reg Address
            RS485_WriteDataBuffer[7 + 1] = (byte)(svregEd / 256);     //Reg Count
            RS485_WriteDataBuffer[8 + 1] = (byte)(svregEd % 256);     //Reg Count        
            funSendMessageTo();
        }
    }
}
