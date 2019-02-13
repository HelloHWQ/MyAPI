using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Models;

namespace MyAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly BloggingContext _context;

        public PostController(BloggingContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// 获取所有帖子的信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetPosts()
        {
            return _context.Post.ToList();
        }

        /// <summary>
        /// 通过博客id获取所有帖子的信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("{blogid}")]
        public ActionResult<IEnumerable<Post>> GetPosts(int blogid)
        {
            var blog = _context.Blog.Find(blogid);
            if(blog == null)
            {
                return NotFound();
            }

            var postlist = (from p in _context.Post
                           where p.BlogId == blogid
                           orderby p.PostId descending
                           select p).ToList<Post>();

            return postlist;
        }

        /// <summary>
        /// 为博客添加帖子
        /// </summary>
        /// <param name="blogid">对应的博客id</param>
        /// <param name="post">帖子json对象</param>
        /// <returns></returns>
        [HttpPost("{blogid}")]
        public ActionResult<Post> PostPost(int blogid, Post post)
        {
            if(blogid != post.BlogId)
            {
                return BadRequest();
            }

            _context.Post.Add(post);
            _context.SaveChanges();
            return Ok(post);
        }

        /// <summary>
        /// 更新帖子但不能改变帖子所属的博客
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPut("{postid}")]
        public IActionResult PutPost(int postid, Post post)
        {
            if(postid != post.PostId)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;
            _context.Entry(post).Property(p => p.BlogId).IsModified = false;
            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// 通过帖子的主键删除帖子
        /// </summary>
        /// <param name="postid"></param>
        /// <returns></returns>
        [HttpDelete("{postid}")]
        public IActionResult DeletePost(int postid)
        {
            var post = _context.Post.Find(postid);

            if(post == null)
            {
                return BadRequest();
            }

            _context.Post.Remove(post);
            _context.SaveChanges();
            return Ok();
        }
    }
}