using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIOMETRICATTENDANCE
{
    class GetVals
    {
        static string unm,fnm, sx, ag, eml, cnt, usr, add, econtt, eaddd;
        public static string ename
        {
            get
            {
                return unm;
            }
            set
            {
                unm = value;
            }
        }
        public static string fname
        {
            get
            {
                return fnm;
            }
            set
            {
                fnm = value;
            }
        }
        public static string sex
        {
            get
            {
                return sx;
            }
            set
            {
                sx = value;
            }
        }
        public static string age
        {
            get
            {
                return ag;
            }
            set
            {
                ag = value;
            }
        }
        public static string Email
        {
            get
            {
                return eml;
            }
            set
            {
                eml = value;
            }
        }
        public static string contn
        {
            get
            {
                return cnt;
            }
            set
            {
                cnt = value;
            }
        }
        public static string user
        {
            get
            {
                return usr;
            }
            set
            {
                usr = value;
            }
        }
        public static string adr
        {
            get
            {
                return add;
            }
            set
            {
                add = value;
            }
        }
        public static string eadd
        {
            get
            {
                return eaddd;
            }
            set
            {
                eaddd = value;
            }
        }
        public static string econt
        {
            get
            {
                return econtt;
            }
            set
            {
                econtt = value;
            }
        }
    }
}
