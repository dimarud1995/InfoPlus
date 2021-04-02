using InfoPlus.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InfoPlus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoatuuController : ControllerBase
    {
        private readonly DBContext db;
        private readonly IWebHostEnvironment environment;

        public KoatuuController(DBContext _db, IWebHostEnvironment _environment)
        {
            db = _db;
            environment = _environment;
        }
        [Route("getKoatuu/{id?}")]
        public IActionResult GetKoatuu(string startWith,int? level,string filterName, string filterType)
        {
            if (startWith == null)
            {
                var list = db.Koatuu.ToList();
                if (filterType != null)
                {
                    list = list.Where(e => e?.NP != null && e.NP.ToLower().Contains(filterType.ToLower())).ToList();
                }

                if (filterName != null)
                {
                    list = list.Where(e =>e?.NP!=null&&e?.NU!=null && e.NU.ToLower().Contains(filterName.ToLower())).ToList();
                }
              var rootList=db.Koatuu.Where(e => e.TE.EndsWith("00000000")).ToList();
                list = list.Join(rootList, eo => eo.TE.Substring(0, 2), ei => ei.TE.Substring(0, 2), (eo, ei) => ei).ToList();
                rootList = rootList.Join(list, eo => eo.TE.Substring(0, 2), ei => ei.TE.Substring(0, 2), (eo, ei) => ei).Distinct().ToList();
                return Ok(rootList);
            }
            else
            {
                if (level != null)
                {
                    int checkFirstNElements = 2;
                    switch (level)
                    {
                        case 1:
                            checkFirstNElements = 2;
                            break;
                        case 2:
                            checkFirstNElements = 3;
                            break;
                        case 3:
                            checkFirstNElements = 5;
                            break;
                        case 4:
                            checkFirstNElements = 6;
                            break;
                        case 5:
                            checkFirstNElements = 8;
                            break;
                        case 6:
                            checkFirstNElements = 10;
                            break;
                        case 7:
                            checkFirstNElements = 10;
                            break;
                        default:
                            checkFirstNElements = 2;
                            break;
                         
                    }
                    var list = db.Koatuu.ToList();
                    if (filterType != null)
                    {
                        list = list.Where(e => e?.NP != null && e.NP.ToLower().Contains(filterType.ToLower())).ToList();
                    }
                    if (filterName != null)
                    {
                        list = list.Where(e => e?.NP!=null &&e?.NU != null && e.NU.ToLower().Contains(filterName.ToLower())).ToList();
                    }
                    list = list.Where(e => e?.NP!=null&&e.TE.StartsWith(startWith)).ToList();
                   var tempStartWidth = startWith;
                    while (tempStartWidth.Length < 10)
                    {
                        tempStartWidth += "0";
                    }
                    var rootList = db.Koatuu.Where(e=>!e.TE.Contains(tempStartWidth)&&e.TE.StartsWith(startWith)&&e.TE.EndsWith(new String('0',10-checkFirstNElements))).ToList();
                    /*  list = list.Except(rootList).ToList();


                     if (rootList.Count == 0)
                     {
                         int n = 0;
                         rootList = db.Koatuu.Where(e => e.NP == null && e.TE.StartsWith(startWith)).ToList();
                         rootList = rootList.Except(rootList.Where(e => e.TE == tempStartWidth)).ToList();
                         var tempRootList = new List<Koatuu>();
                         do
                         {
                             tempRootList = rootList.Join(list, eo => eo.TE[0..n], ei => ei.TE[0..n], (eo, ei) => eo).Distinct().ToList();
                             n++;
                         } while (tempRootList.Count == 0 && n <= 10);
                         rootList = tempRootList;
                     }*/
                    var tempList = list.Join(rootList, eo => eo.TE[0..checkFirstNElements], ei => ei.TE[0..checkFirstNElements], (eo, ei) => ei).ToList();
                    if (tempList.Count == 0 && list.Count > 0)
                    {
                        return Ok(list);
                    }
                    rootList = rootList.Join(tempList, eo => eo.TE[0..checkFirstNElements], ei => ei.TE[0..checkFirstNElements], (eo, ei) => eo).Distinct().ToList();
                    return Ok(rootList);
                }
                else
                {
                    var list = db.Koatuu.ToList();
                    if (filterType != null)
                    {
                        list = list.Where(e => e?.NP != null && e.NP.ToLower().Contains(filterType.ToLower())).ToList();
                    }
                    if (filterName != null)
                    {
                        list = list.Where(e => e?.NP != null && e?.NU != null && e.NU.ToLower().Contains(filterName.ToLower())).ToList();
                    }
                   
                    if (filterName == null && filterType == null) { 
                        list=list.Where(e => e.TE.EndsWith("00000000")).ToList();
                    }
                    return Ok(list);
                }
            }
        }
        [Route("addKoatuu")]
        public IActionResult AddKoatuu([FromForm] string TE, [FromForm] string NP, [FromForm] string NU) 
        {
            try
            {
                if(db.Koatuu.FirstOrDefault(e => e.TE == TE)!=null)
                {
                    return Ok("Такий ТЕ уже існує!");
                }
                var koatuu = new Koatuu
                {
                   TE=TE,
                   NP=NP,
                   NU=NU
                };
                db.Koatuu.Add(koatuu);
                db.SaveChanges();
                return Ok("Saved!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Route("removeKoatuu/{id?}")]
        public IActionResult RemoveKoatuu(int id)
        {
            try
            {
                var tempKoatuu = db.Koatuu.FirstOrDefault(e => e.Id == id);
                if (tempKoatuu != null)
                {
                    db.Koatuu.Remove(tempKoatuu);
                    db.SaveChanges();
                }
                return Ok("Deleted!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Route("updateKoatuu")]
        public IActionResult UpdateKoatuu([FromForm] int ID,[FromForm] string TE, [FromForm] string NP, [FromForm] string NU)
        {
            try
            {
                var koatuu = db.Koatuu.FirstOrDefault(e => e.Id == ID);
                if (koatuu == null) {
                    return Ok("Елементу з таким ID не істує!");
                 }
                koatuu.TE = TE;
                koatuu.NP = NP;
                koatuu.NU = NU;
                db.Koatuu.Update(koatuu);
                db.SaveChanges();
                return Ok("Збережено!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        //
        // This method only for ititializing db
        //
        [Route("uploaddb")]

        public  IActionResult UploadDB()
        {
                List<Koatuu> items = new List<Koatuu>();
                var path = Path.Combine(environment.WebRootPath, "data.json");
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    items = JsonConvert.DeserializeObject<List<Koatuu>>(json);
                }
            if (db.Koatuu.FirstOrDefault(e => e.TE == items[0].TE) == null)
            {
                db.Koatuu.AddRange(items);
                db.SaveChanges();
                return Ok("Saved!");
            }
            return Ok("Already saved!");
        }
    }
}
