using Newtonsoft.Json.Linq;
using Prism.Commands;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SchoolMeal;

namespace LINDA
{
    /// <summary>
    /// Meal.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Meal : UserControl
    {
        string schoolcode = null;
        string schoollocate = null;
        string officecode = null;
        string schoolnameall = null;
        Regions regionsall;
        SchoolType type;
        string SCHOOLURL = "";
        string SCHOOLNAMEURL = "/searchschool?school=-";
        string MEALURL = "";
        public Meal()
        {
            InitializeComponent();
            DataContext = this;
            EnterProgramCommand = new DelegateCommand(GetData);
        }

        public ICommand EnterProgramCommand
        {
            get;
            set;
        }

        private void GetData()
        {
            GetSchoolCode(tbxInsert.Text);
            GetRegions();
            JudgmentLevel();
            SchoolMeal.Meal meal = new SchoolMeal.Meal(regionsall, type, officecode + schoolcode);
            var menus = meal.GetMealMenu();
            foreach(var menu in menus)
            {
                int buffer = DateTime.Compare(DateTime.Today, menu.Date);
                if(buffer == 0)
                {
                    if (menu.Breakfast == null) tbBreakfast.Text = "급식이 등록되어있지 않습니다";
                    else tbBreakfast.Text = string.Join("\n", menu.Breakfast.ToArray());
                    if (menu.Lunch == null) tbLunch.Text = "급식이 등록되어있지 않습니다";
                    else tbLunch.Text = string.Join("\n", menu.Lunch.ToArray());
                    if (menu.Dinner == null) tbDinner.Text = "급식이 등록되어있지 않습니다";
                    else tbDinner.Text = string.Join("\n", menu.Dinner.ToArray());
                    break;
                }
            }
        }
        private void GetSchoolCode(string schoolname)
        {
            var client = new RestClient(SCHOOLURL);
            var request = new RestRequest(SCHOOLNAMEURL.Replace("-", schoolname), Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            JToken jtoken = JToken.Parse(content);
            jtoken = jtoken["data"];
            jtoken = jtoken["schoolInfo"];
            string a = jtoken[0]["schoolCode"].ToString();
            string b = jtoken[0]["schoolLocate"].ToString();
            string c = jtoken[0]["officeCode"].ToString();
            string d = jtoken[0]["schoolName"].ToString();
            schoolcode = a;
            schoollocate = b;
            officecode = c;
            schoolnameall = d;

        }
        private void JudgmentLevel()
        {
            if (schoolnameall.Contains("고등학교"))
            {
                type = SchoolType.High;
            }
            else if (schoolnameall.Contains("중학교"))
            {
                type = SchoolType.Middle;
            }
            else if (schoolnameall.Contains("초등학교"))
            {
                type = SchoolType.Elementary;
            }
            else if(schoolnameall.Contains("대학교"))
            {
                type = SchoolType.Kindergarden;
            }
        }
        private void GetRegions()
        {
            string[] items = schoollocate.Split(' ');
            if (items[0] == "서울특별시") regionsall = Regions.Seoul;
            else if (items[0] == "인천광역시") regionsall = Regions.Incheon;
            else if (items[0] == "부산광역시") regionsall = Regions.Busan;
            else if (items[0] == "광주광역시") regionsall = Regions.Gwangju;
            else if (items[0] == "대전광역시") regionsall = Regions.Daejeon;
            else if (items[0] == "대구광역시") regionsall = Regions.Daegu;
            else if (items[0] == "세종특별자치시") regionsall = Regions.Sejong;
            else if (items[0] == "울산광역시") regionsall = Regions.Ulsan;
            else if (items[0] == "경기도") regionsall = Regions.Gyeonggi;
            else if (items[0] == "강원도") regionsall = Regions.Kangwon;
            else if (items[0] == "충청북도") regionsall = Regions.Chungbuk;
            else if (items[0] == "청청남도") regionsall = Regions.Chungnam;
            else if (items[0] == "경상북도") regionsall = Regions.Gyeongbuk;
            else if (items[0] == "경상남도") regionsall = Regions.Gyeongnam;
            else if (items[0] == "전라북도") regionsall = Regions.Jeonbuk;
            else if (items[0] == "전라남도") regionsall = Regions.Jeonnam;
            else if (items[0] == "제주특별자치도") regionsall = Regions.Jeju;

        }
    }


}
