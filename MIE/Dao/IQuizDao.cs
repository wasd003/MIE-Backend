﻿using System;
using System.Collections.Generic;
using MIE.Entity;

namespace MIE.Dao
{
    public interface IQuizDao
    {
        Quiz GetQuizById(int id);

        List<Quiz> GetQuizByPageId(int pageId);

        List<Quiz> GetAllQuizzes();
    }
}
