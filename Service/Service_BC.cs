using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Carrier.Model.BC_TB;

namespace Carrier.Service
{
    public class Service_BC
    {
        BC_TBEntities bC_TB_Entities = new BC_TBEntities();
        public List<BC_Dimension_Values> getDimensionValue(string DimensionCode)
        {
            var dimens = bC_TB_Entities.BC_Dimension_Values.Where(w => w.Dimension_Code == DimensionCode).ToList();
            return dimens;
        }

        

    }


    
}