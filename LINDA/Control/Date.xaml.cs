using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LINDA
{
    /// <summary>
    /// Date.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Date : UserControl
    {
        List<schoolInfo> schools = new List<schoolInfo>();

        string realid = null;
        string SCHOOLURL = "";
        string ADDURL = "/calendar/add";
        string DELETEURL = "/calendar/remove/-";
        string SCHOOLNAMEURL = "/searchschool?school=-";
        string DATEURL = "";
        public Date()
        {
            InitializeComponent();
            initDateplan();
        }
        private JObject Makedata(List<string> data)
        {
            JObject jObject = new JObject();
            jObject["date"] = data[0];
            jObject["content"] = data[1];
            jObject["writer"] = data[2];
            jObject["school"] = data[3];
            jObject["grade"] = data[4];
            jObject["class"] = data[5];
            jObject["schoolcode"] = GetSchoolCode(data[3]);

            return jObject;
        }

        private string GetSchoolCode(string schoolname)
        {
            var client = new RestClient(SCHOOLURL);
            var request = new RestRequest(SCHOOLNAMEURL.Replace("-", schoolname), Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            JToken jtoken = JToken.Parse(content);
            jtoken = jtoken["data"];
            jtoken = jtoken["schoolInfo"];
            string a = jtoken[0]["schoolCode"].ToString();
            return a;
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            JObject json;
            List<string> data = new List<string>();
            string day = DateTime.Parse(clDate.SelectedDate.ToString()).ToString("yyyyMMdd");
            data.Add(day);
            data.Add(tbxContent.Text);
            data.Add(tbxWriter.Text);
            
            data.Add(tbxSchool.Text);
            if (tbxGrade.Text == "") data.Add(null);
            else data.Add(tbxGrade.Text);
            if (tbxClass.Text == "") data.Add(null);
            else data.Add(tbxClass.Text);
            json = Makedata(data);
            
            var client = new RestClient(SCHOOLURL);
            var request = new RestRequest(ADDURL, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            tbxClass.Text = "";
            tbxContent.Text = "";
            tbxGrade.Text = "";
            tbxSchool.Text = "";
            tbxTitle.Text = "";
            tbxWriter.Text = "";
            initDateplan();

            MessageBox.Show("일정이 추가되었습니다");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string day = DateTime.Parse(clDate.SelectedDate.ToString()).ToString("yyyy-MM-dd");
            string url = DELETEURL.Replace("-", realid);
            var client = new RestClient(SCHOOLURL);
            var request = new RestRequest(url, Method.DELETE);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            tbxClass.Text = "";
            tbxContent.Text = "";
            tbxGrade.Text = "";
            tbxSchool.Text = "";
            tbxTitle.Text = "";
            tbxWriter.Text = "";
            initDateplan();
            MessageBox.Show("일정이 삭제되었습니다");

        }
       
        private void ClDate_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            initDateplan();
        }

        private void initDateplan()
        {
            string year = DateTime.Parse(clDate.DisplayDate.ToString()).ToString("yyyy");
            string month = DateTime.Parse(clDate.DisplayDate.ToString()).ToString("MM");
            string url = DATEURL.Replace("_", year);
            url = url.Replace("-", month);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string json = wc.DownloadString(url);
            wc.Dispose();

            JToken obj = JToken.Parse(json);
            obj = obj["data"];
            obj = obj["calendarSchedule"];
            schools.RemoveRange(0, schools.Count);
            foreach (var item in obj)
            {
                schools.Add(new schoolInfo
                {
                    date = item["date"].ToString(),
                    id = item["id"].ToString(),
                    content = item["content"].ToString()
                });
            }
            ChangeColor();
        }

        private void ClDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            
            string day = DateTime.Parse(clDate.SelectedDate.ToString()).ToString("yyyy-MM-dd");
            foreach (var item in schools)
            {
                if (day == item.date)
                {
                    realid = item.id;
                    tbxContent.Text = item.content;
                    break;
                }
            }
            ChangeColor();
        }

        private void ChangeColor()
        {
            DateTime buffer;
            foreach (var item in schools)
            {
                buffer = DateTime.Parse(item.date);
                clDate.SelectedDates.Add(buffer);
            }
        }
    }
}
