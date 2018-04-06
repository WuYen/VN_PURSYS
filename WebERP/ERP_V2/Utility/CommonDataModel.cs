using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2
{
    public class CommonDataModel
    {
        private static List<BA01A> _BA01A;
        public static List<BA01A> BA01AList
        {
            get
            {
                if (_BA01A == null)
                {
                    var entity = new PURSysEntities();
                    _BA01A = entity.BA01A.ToList();
                }
                return _BA01A;
            }
            set
            {
                var entity = new PURSysEntities();
                _BA01A = entity.BA01A.ToList();
            }
        }

        private static List<BA02A> _BA02A;
        public static List<BA02A> BA02AList
        {
            get
            {
                if (_BA02A == null)
                {
                    var entity = new PURSysEntities();
                    _BA02A = entity.BA02A.ToList();
                }
                return _BA02A;
            }
            set
            {
                var entity = new PURSysEntities();
                _BA02A = entity.BA02A.ToList();
            }
        }

        private static List<BA03A> _BA03A;
        public static List<BA03A> BA03AList
        {
            get
            {
                if (_BA03A == null)
                {
                    var entity = new PURSysEntities();
                    _BA03A = entity.BA03A.ToList();
                }
                return _BA03A;
            }
            set
            {
                var entity = new PURSysEntities();
                _BA03A = entity.BA03A.ToList();
            }
        }

        //private static List<VW_BA01A> _VW_BA01A;
        //public static List<VW_BA01A> VW_BA01AAList
        //{
        //    get
        //    {
        //        if (_VW_BA01A == null)
        //        {
        //            var entity = new PURSysEntities();
        //            _VW_BA01A = entity.VW_BA01A.ToList();
        //        }
        //        return _VW_BA01A;
        //    }
        //    set
        //    {
        //        var entity = new PURSysEntities();
        //        _VW_BA01A = entity.VW_BA01A.ToList();
        //    }
        //}
    }
}