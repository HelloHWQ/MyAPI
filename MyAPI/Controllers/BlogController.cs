using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly BloggingContext _context;

        public BlogController(BloggingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有的博客信息列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Blog>> GetBlogs()
        {
            //JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.ContractResolver = new DefaultContractResolver();

            JsonResult jr = new JsonResult(_context.Blog.Include(b => b.Post).ToList());
            return jr;
            //return _context.Blog.Include(b => b.Post).ToList();
        }

        /// <summary>
        /// 分页获取博客信息列表
        /// </summary>
        /// <param name="pageindex">当前页</param>
        /// <param name="pagesize">页大小</param>
        /// <returns></returns>
        [HttpGet("{pageindex}/{pagesize}")]
        public ActionResult<IEnumerable<Blog>> GetBlogsWithPage(int pageindex, int pagesize)
        {
            if(pagesize == 0)
            {
                pagesize = 3;
            }

            var bloglist = (from b in _context.Blog
                            orderby b.Url ascending, b.BlogId descending
                            select b).ToList();
            var pagelist = bloglist.Skip((pageindex - 1) * pagesize).Take(pagesize);

            return Ok(pagelist);
        }

        /// <summary>
        /// 关键字查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("/api/blog/search/{keyword}")]
        public ActionResult<IEnumerable<Blog>> GetBlogsWithKeyword(string keyword)
        {
            if(!string.IsNullOrEmpty(keyword))
            {
                var bloglist = from b in _context.Blog
                               where b.Url.Contains(keyword)
                               orderby b.BlogId ascending
                               select b;
                return bloglist.ToList();
            }
            else
            {
                return _context.Blog.ToList();
            }
        }

        /// <summary>
        /// 根据提供的博客id返回单个博客信息
        /// </summary>
        /// <param name="blogid">博客id</param>
        /// <returns></returns>
        [HttpGet("{blogid}")]
        public ActionResult<Blog> GetBlog(int blogid)
        {
            var blog = _context.Blog.Find(blogid);

            if (blog == null)
            {
                return NotFound();
            }

            return Ok(blog);
        }

        /// <summary>
        /// 增加一个博客信息
        /// </summary>
        /// <param name="blog">博客信息json对象</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /blog
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<Blog> PostBlog(Blog blog)
        {
            if (blog == null)
            {
                return BadRequest();
            }

            _context.Blog.Add(blog);
            _context.SaveChanges();
            return Ok(blog);
        }

        /// <summary>
        /// 整体更新博客信息
        /// </summary>
        /// <param name="blogid">想要更新的博客信息id</param>
        /// <param name="blog">更新之后的博客信息</param>
        /// <returns></returns>
        [HttpPut("{blogid}")]
        public IActionResult PutBlog(int blogid, Blog blog)
        {
            if(blog.BlogId != blogid)
            {
                return BadRequest();
            }

            _context.Entry(blog).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// 部分更新博客信息
        /// </summary>
        /// <param name="blogid">想要更新的博客信息id</param>
        /// <param name="blog">更新之后的博客信息</param>
        /// <returns></returns>
        [HttpPatch("{blogid}")]
        public IActionResult PathBlog(int blogid, Blog blog)
        {
            if (blog.BlogId != blogid)
            {
                return BadRequest();
            }

            _context.Blog.Update(blog);
            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// 删除一个博客信息
        /// </summary>
        /// <param name="blogid">要删除的博客信息id</param>
        /// <returns></returns>
        [HttpDelete("{blogid}")]
        public IActionResult DeleteBlog(int blogid)
        {
            var blog = _context.Blog.Find(blogid);

            if(blog == null)
            {
                return NotFound();
            }

            _context.Entry(blog).State = EntityState.Deleted;
            _context.SaveChanges();
            return NoContent();
        }
    }
}