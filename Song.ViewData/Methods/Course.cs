﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Song.Entities;
using Song.ServiceInterfaces;
using Song.ViewData.Attri;
using WeiSha.Common;


namespace Song.ViewData.Methods
{
    public class Course
    {
        [HttpGet(IsAllow = false)]
        public Song.Entities.Course ForID(int id)
        {
            return Business.Do<ICourse>().CourseSingle(id);
        }
    }
}
