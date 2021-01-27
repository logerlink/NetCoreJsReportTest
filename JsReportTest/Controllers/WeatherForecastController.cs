using jsreport.AspNetCore;
using jsreport.Types;
using JsReportTest.Filters;
using JsReportTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JsReportTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : Controller
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("barCodeTest")]
        [MiddlewareFilter(typeof(JsReportPipeline))]
        public IActionResult BarCodeTest()
        {
            HttpContext.JsReportFeature().Recipe(Recipe.ChromePdf);
            HttpContext.JsReportFeature().OnAfterRender((r) => {
                using (var file = System.IO.File.Open("report.pdf", FileMode.Create))
                {
                    r.Content.CopyTo(file);
                }
                r.Content.Seek(0, SeekOrigin.Begin);
            });
            return Json("11111111111111");
        }

        /// <summary>
        /// 打印标签
        /// </summary>
        /// <returns></returns>
        [HttpGet("label")]
        //jsReport支持
        [MiddlewareFilter(typeof(JsReportPipeline))]
        public IActionResult Label()
        {
            //jsReport支持
            HttpContext.JsReportFeature().Recipe(Recipe.ChromePdf); //Recipe.ChromePdf 即模板
            return View(Init());
        }

        /// <summary>
        /// 打印标签并保存
        /// </summary>
        /// <returns></returns>
        [HttpGet("labelAndSave")]
        //jsReport支持
        [MiddlewareFilter(typeof(JsReportPipeline))]
        public IActionResult LabelAndSave()
        {
            //jsReport支持
            HttpContext.JsReportFeature().Recipe(Recipe.ChromePdf).OnAfterRender((res)=> {
                var fileName = "条形码_" + DateTime.Now.ToString("yyMMddHHmmss");
                using (var file = System.IO.File.Open(fileName+".pdf", FileMode.Create))
                {
                    res.Content.CopyTo(file);
                }
                res.Content.Seek(0, SeekOrigin.Begin);
                //todo 发邮件
            });
            return View("Label",Init());
        }

        /// <summary>
        /// 打印标签并筛选结果集
        /// </summary>
        /// <returns></returns>
        [HttpGet("labelFilterTest")]
        //jsReport支持
        [MiddlewareFilter(typeof(JsReportPipeline))]
        //此处Filter作用不大，如果将此Filter用于修饰Class，效果会明显一点，此处不演示
        [TokenFilter]   
        public IActionResult LabelFilterTest()
        {
            //jsReport支持
            HttpContext.JsReportFeature().Recipe(Recipe.ChromePdf); //Recipe.ChromePdf 即模板
            return View("Label",Init());
        }
        /// <summary>
        /// 生成ViewModel
        /// </summary>
        /// <returns></returns>
        private LabelModel Init()
        {
            var labelModel = new LabelModel
            {
                Fnsku = "X003O97GHR"
            };
            if (string.IsNullOrWhiteSpace(labelModel.Fnsku)) throw new Exception("Fnsku不可未空");
            var fileName = "条形码_" + labelModel.Fnsku + DateTime.Now.ToString("yyMMddHHmmss");
            var path = AppDomain.CurrentDomain.BaseDirectory + "Files\\pdfTemp\\" + fileName + ".jpg";
            Util.CreateBarcode(labelModel.Fnsku, 320, 37, path);
            labelModel.Src = Util.ImageToBase64(path);
            var title = "Amazon Basics 48 Pack AA High-Performance Alkaline Batteries, 10-Year Shelf Life, Easy to Open Value Pack";
            if (!string.IsNullOrWhiteSpace(title) && title.Length > 18 && title.Length <= 35) labelModel.Title = title.Substring(0, 18) + "...," + title[^35..];
            else if (!string.IsNullOrWhiteSpace(title) && title.Length > 35) labelModel.Title = title.Substring(0, 18) + "...," + title[^35..];
            else labelModel.Title = title;

            //if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

            return labelModel;
        }

    }
}
