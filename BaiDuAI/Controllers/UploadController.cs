using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace BaiDuAI.Controllers
{
    public class UploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private JsonResultModel uplode(IFormFile uploadfile,string filePath,string webRootPath)
        {

            if (!Directory.Exists(webRootPath + filePath))
            {
                Directory.CreateDirectory(webRootPath + filePath);
            }
            if (uploadfile != null)
            {
                //文件后缀
                var fileExtension = Path.GetExtension(uploadfile.FileName);

                //判断后缀是否是图片
                const string fileFilt = ".jpg|.jpeg|.png|.bmp";
                if (fileExtension == null)
                {
                    return new JsonResultModel { isSucceed = false, resultMsg = "上传的文件没有后缀" };
                }
                if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                {
                    return new JsonResultModel { isSucceed = false, resultMsg = "仅支持"+ fileFilt+"类型图片" };
                }

                //判断文件大小    
                long length = uploadfile.Length;
                if (length > 1024 * 1024 * 4) //2M
                {
                    return new JsonResultModel { isSucceed = false, resultMsg = "上传的文件不能大于4M" };
                }

                var strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                var strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数
                var saveName = strDateTime + strRan + fileExtension;


                using (FileStream fs = System.IO.File.Create(webRootPath + filePath + saveName))
                {
                    uploadfile.CopyTo(fs);
                    fs.Flush();
                }
                
                return new JsonResultModel { isSucceed = true, resultMsg = "上传成功",Date= webRootPath + filePath + saveName };
            }
            return new JsonResultModel { isSucceed = false, resultMsg = "上传失败" };
           
        }
        public ActionResult Idcard([FromServices]IHostingEnvironment env)
        {
            var id_card_side = Request.Form["id_card_side"];
            string id_card_sidep = "back";
           
            if (id_card_side.Count>0)
            {
                id_card_sidep = "front";
            }
            var now = DateTime.Now;
            var uploadfile = Request.Form.Files[0];
          
            var   webRootPath = env.WebRootPath;
            var filePath = string.Format("/Uploads/Images/{0}/{1}/{2}/", now.ToString("yyyy"), now.ToString("yyyyMM"), now.ToString("yyyyMMdd"));

            Stopwatch w1 = new Stopwatch();
            w1.Start();
            var  result= uplode(uploadfile, filePath, webRootPath);
            w1.Stop();
            var message = "上传图片耗时：【" + w1.Elapsed.TotalMilliseconds + "】毫秒\r\n";
            if (result.isSucceed)
            {
                Stopwatch w3 = new Stopwatch();
                w3.Start();
                var img = System.IO.File.ReadAllBytes(result.Date.ToString());
                message += "加载图片耗时：【" + w3.Elapsed.TotalMilliseconds + "】毫秒\r\n";
                w3.Stop();
                Stopwatch w2 = new Stopwatch();
                w2.Start();
                var s = BaiDuAIHelper.BaiDuAIHelper.Idcard(img, id_card_sidep);
                message += "百度识别图片耗时：【" + w2.Elapsed.TotalMilliseconds + "】毫秒\r\n";
                message += "接口返回结果：";
                w2.Stop();
                result.Date = message + s;
                return Json(result); 
            }
            else
            {
                return Json(result);
            }


        }


        public ActionResult BusinessLicense([FromServices]IHostingEnvironment env)
        {
           
            var now = DateTime.Now;
            var uploadfile = Request.Form.Files[0];

            var webRootPath = env.WebRootPath;
            var filePath = string.Format("/Uploads/Images/{0}/{1}/{2}/", now.ToString("yyyy"), now.ToString("yyyyMM"), now.ToString("yyyyMMdd"));

            Stopwatch w1 = new Stopwatch();
            w1.Start();
            var result = uplode(uploadfile, filePath, webRootPath);
            w1.Stop();
            var message = "上传图片耗时：【" + w1.Elapsed.TotalMilliseconds + "】毫秒\r\n";
            if (result.isSucceed)
            {
                Stopwatch w3 = new Stopwatch();
                w3.Start();
                var img = System.IO.File.ReadAllBytes(result.Date.ToString());
                message += "加载图片耗时：【" + w3.Elapsed.TotalMilliseconds + "】毫秒\r\n";
                w3.Stop();
               
                Stopwatch w2 = new Stopwatch();
                w2.Start();
                var s = BaiDuAIHelper.BaiDuAIHelper.BusinessLicense(img);
                w2.Stop();
                message += "百度识别图片耗时：【" + w2.Elapsed.TotalMilliseconds + "】毫秒\r\n";
                message += "接口返回结果：";
                
                result.Date = message + s;
              

              
             
                return Json(result);
            }
            else
            {
                return Json(result);
            }


        }
    }
}