using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSProject
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> staff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;

            while(year == 0)
            {
                Console.Write("\nPlease enter the year: ");

                try
                {
                    string temp = Console.ReadLine();
                    year = Convert.ToInt32(temp);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error");
                }
            }

            while (month == 0 || month > 12)
            {
                Console.Write("\nPlease enter the year: ");

                try
                {
                    string temp = Console.ReadLine();
                    month = Convert.ToInt32(temp);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error");
                }
            }

            var myStaff = fr.ReadFile();

            for(int i=0; i<myStaff.Count(); i++)
            {
                try
                {
                    Console.Write("Enter hours worked for " + myStaff[i].NameOfStaff + ":");
                    string hours = Console.ReadLine();
                    myStaff[i].HoursWorked = Convert.ToInt32(hours);
                    myStaff[i].CalculatePay();

                    var wri = myStaff[i].ToString();
                    Console.WriteLine(wri);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error");
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.ReadKey();
        }
    }

    class Staff
    {
        private float hourlyRate;
        private int hWorked;

        public Staff(string name, float rate)
        {

        }

        public override string ToString()
        {
            return BasicPay.ToString() + " " + TotalPay.ToString();
        }

        public virtual void CalculatePay()
        {
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public float TotalPay
        {
            get
            {
                return TotalPay;
            }
            set
            {
                TotalPay = value;
            }
        }


        public float BasicPay
        {
            get
            {
                return BasicPay;
            }
            set
            {
                BasicPay = value;
            }
        }

        public string NameOfStaff
        {
            get
            {
                return NameOfStaff;
            }
            set
            {
                NameOfStaff = value;
            }
        }

        public int HoursWorked
        {
            get
            {
                return hWorked;
            }
            set
            {
                if(hWorked == 0)
                {
                    hWorked = 0;
                }else
                {
                    hWorked = value;
                }
            }
        }
    }

    class Manager : Staff 
    {
        private const float managerHourlyRate = 50;

        public Manager(string name) : base(name, managerHourlyRate)
        {

        }

        public int Allowance { get; set; }

        public override void CalculatePay()
        {
            base.CalculatePay();
            Allowance = 1000;

            if(HoursWorked >= 160)
            {
                TotalPay = Allowance;
            }
        }

        public override string ToString()
        {
            return TotalPay.ToString();
        }
    }

    class Admin : Staff
    {
        public float Overtime { get; set; }

        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30;

        public Admin(string name) : base(name, adminHourlyRate)
        {

        }

        public override void CalculatePay()
        {
            Overtime = overtimeRate * (HoursWorked - 160);

            if (Overtime > 160)
            {
                TotalPay = TotalPay + Overtime;
            }
        }

        public override string ToString()
        {
            return Overtime.ToString();
        }
    }

    class FileReader
    {
        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] separator = { ", " };

            if (File.Exists("staff.txt"))
            {
                using(StreamReader sr = new StreamReader(path))
                {
                    string s;
                    while (!sr.EndOfStream)
                    {
                        s = sr.ReadLine();
                        result = s.Split(separator, StringSplitOptions.None);
                        
                        if(result[1].GetType() == typeof(Manager))
                        {
                            Manager mn = new Manager(result[0]);
                            myStaff.Add(mn);
                        }else if(result[1].GetType() == typeof(Admin))
                        {
                            Admin ad = new Admin(result[0]);
                            myStaff.Add(ad);
                        }
                    }
                    sr.Close();
                }
            }

            return null;
        }
    }

    class PaySlip
    {
        private enum MonthOfYears { JAN, FEB, MAR, APR, MEI, JUN, JUL, AGU, SEP, OKT, NOV, DES };

        private int month;
        private int year;

        public PaySlip(int payMonth, int payYear)
        {
            payMonth = month;
            payYear = year;
        }

        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string path;

            foreach (Staff f in myStaff)
            {
                path = f.NameOfStaff + ".txt";
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("PAYSLIP FOR $ {0} {1}", (MonthOfYears)month, year);
                sw.WriteLine("==============");
                sw.WriteLine("Name of Staff: $ {0}", f.NameOfStaff);
                sw.WriteLine("Hours Worked: $ {0}", f.HoursWorked);
                sw.WriteLine("");
                sw.WriteLine("Basic Pay: $ {0}", f.BasicPay);

                if(f.GetType() == typeof(Manager))
                {
                    sw.WriteLine("Allowance: $ {0}", ((Manager)f).Allowance);
                }else if (f.GetType() == typeof(Admin))
                {
                    sw.WriteLine("Allowance: $ {0}", ((Admin)f).Overtime);
                }

                sw.WriteLine("");
                sw.WriteLine("==============");
                sw.WriteLine("Total Pay: $ {0}", f.TotalPay);
                sw.WriteLine("==============");
                sw.Close();
            }
        }
        
        public void GenerateSummary(List<Staff> myStaff)
        {
            string path = "summary.txt";

            foreach(Staff f in myStaff)
            {
                //var result = myStaff.OrderBy(d => d).ToList();
                var result = from s in myStaff where s.HoursWorked < 10 orderby s select s;

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");

                foreach(var temp in result)
                {
                    sw.WriteLine("Name of Staff: " + temp.NameOfStaff + ", Hours Worked: " + temp.HoursWorked);
                }

                sw.Close();
            }
        }

        public override string ToString()
        {
            return "";
        }
    }
}
