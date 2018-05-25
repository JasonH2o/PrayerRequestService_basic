using System;
using System.Linq;
using System.Web.Http;
using PrayerRequest.Service.DataContext;
using PrayerRequest.Service.Models;
using System.Web.Http.Cors;
using PrayerRequest.Service.GenericClass;


namespace PrayerRequest.Service.Controllers
{
    // TODO: modify the origin when deploy to server
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PrayerRequestController : ApiController
    {
        private DatabaseContext dbContext = new DatabaseContext();        
        
        //[EnableQuery(PageSize = 10)]
        //[ResponseType(typeof(IQueryable<PrayerRequestDetail>))]
        public IHttpActionResult GetPrayer(int pageNo = 0, int pageSize = 10)
        {
            try
            {
                // Determine the number of records to skip
                int skip = (pageNo - 1) * pageSize;

                // Group by open prayer first then order by Id
                //var result = dbContext.PrayerRequests.GroupBy(z => z.IsCurrent).SelectMany(x => x).OrderByDescending(grouping=> grouping.Id);
                var result = dbContext.PrayerRequests.ToList().GroupBy(b => b.IsCurrent).Select(grouping => grouping.OrderByDescending(b => b.Id))
                            .OrderByDescending(grouping => grouping.First().IsCurrent)
                            .SelectMany(grouping => grouping)
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                var total = dbContext.PrayerRequests.Count();

                if (!result.Any())
                {
                    return NotFound();
                }
                return Ok(new PagedResult<PrayerRequestDetail>(result, pageNo, pageSize, total));
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult PostPrayer(PrayerRequestDetail request)
        {
            try
            {
                var skip = 0;
                var pageSize = 10;
                var pageNo = 1;

                dbContext.PrayerRequests.Add(request);
                dbContext.SaveChanges();
                //var result = dbContext.PrayerRequests.Select(x => x).OrderByDescending(y => y.Id);
                var result = dbContext.PrayerRequests.ToList().GroupBy(b => b.IsCurrent).Select(grouping => grouping.OrderByDescending(b => b.Id))
                            .OrderByDescending(grouping => grouping.First().IsCurrent)
                            .SelectMany(grouping => grouping)
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                var total = dbContext.PrayerRequests.Count();
                var temp = new PagedResult<PrayerRequestDetail>(result, pageNo, pageSize, total);

                return Ok(new PagedResult<PrayerRequestDetail>(result, pageNo, pageSize, total));
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        
        
        public IHttpActionResult DeletePrayer(int id)
        {
            try
            {
                var skip = 0;
                var pageSize = 10;
                var pageNo = 1;

                var requestToBeDeleted = dbContext.PrayerRequests.FirstOrDefault(x => x.Id == id);

                if (requestToBeDeleted != null)
                {
                    dbContext.Entry(requestToBeDeleted).State = System.Data.Entity.EntityState.Deleted;
                    dbContext.SaveChanges();
                    //var result = dbContext.PrayerRequests.Select(x => x).OrderByDescending(y => y.Id);
                    var result = dbContext.PrayerRequests.ToList().GroupBy(b => b.IsCurrent).Select(grouping => grouping.OrderByDescending(b => b.Id))
                            .OrderByDescending(grouping => grouping.First().IsCurrent)
                            .SelectMany(grouping => grouping)
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                    var total = dbContext.PrayerRequests.Count();
                    return Ok(new PagedResult<PrayerRequestDetail>(result, pageNo, pageSize, total));
                }

                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        
        public IHttpActionResult PutPrayer(int id, PrayerRequestDetail updatedRequest)
        {
            try
            {
                var skip = 0;
                var pageSize = 10;
                var pageNo = 1;

                PrayerRequestDetail existingRequest;
                if (updatedRequest.Id < 0)
                {
                    existingRequest = dbContext.PrayerRequests.FirstOrDefault(x => x.Id == id);
                    existingRequest.IsCurrent = false;
                    dbContext.SaveChanges();
                    var result = dbContext.PrayerRequests.ToList().GroupBy(b => b.IsCurrent).Select(grouping => grouping.OrderByDescending(b => b.Id))
                            .OrderByDescending(grouping => grouping.First().IsCurrent)
                            .SelectMany(grouping => grouping)
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                    var total = dbContext.PrayerRequests.Count();
                    return Ok(new PagedResult<PrayerRequestDetail>(result, pageNo, pageSize, total));

                }

                existingRequest = dbContext.PrayerRequests.FirstOrDefault(x => x.Id == updatedRequest.Id);
                if (existingRequest != null)
                {                    
                    existingRequest.Name = updatedRequest.Name;
                    existingRequest.Request = updatedRequest.Request;
                    existingRequest.IsCurrent = updatedRequest.IsCurrent;
                     
                    dbContext.SaveChanges();
                    var result = dbContext.PrayerRequests.ToList().GroupBy(b => b.IsCurrent).Select(grouping => grouping.OrderByDescending(b => b.Id))
                            .OrderByDescending(grouping => grouping.First().IsCurrent)
                            .SelectMany(grouping => grouping)
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                    var total = dbContext.PrayerRequests.Count();
                    return Ok(new PagedResult<PrayerRequestDetail>(result, pageNo, pageSize, total));
                }

                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
