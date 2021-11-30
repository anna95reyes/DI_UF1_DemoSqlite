using System;
using System.Collections.Generic;
using System.Text;

namespace DBLib
{
    public class Dept
    {
        private int dept_no;
        private string dnom;
        private string loc;

        public Dept(int dept_no, string dnom, string loc)
        {
            Dept_no = dept_no;
            Dnom = dnom;
            Loc = loc;
        }

        public int Dept_no { get => dept_no; set => dept_no = value; }
        public string Dnom { get => dnom; set => dnom = value; }
        public string Loc { get => loc; set => loc = value; }
    }
}
