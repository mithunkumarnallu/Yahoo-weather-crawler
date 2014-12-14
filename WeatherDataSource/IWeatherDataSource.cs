using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataSource
{
    interface IWeatherDataSource
    {
        void LoadDataSource();

        DataTable GetData(string state, string city = null);
    }
}
