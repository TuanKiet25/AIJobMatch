using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Documents;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly ElasticsearchClient _elasticSearchClient;
        public ElasticSearchService(ElasticsearchClient elasticSearchClient)
        {
            _elasticSearchClient = elasticSearchClient;
        }
        public async Task<bool> IndexJobAsync(JobPostingDocument job)
        {
            var response = await _elasticSearchClient.IndexAsync(job, idx => idx.Index("job_postings"));
            return response.IsValidResponse;
        }

        public async Task<List<JobPostingDocument>> RecommendJobsAsync(JobSearchRequest jobSearchRequest)
        {

            var shouldQueries = new List<Query>();

            if (jobSearchRequest.candidateSkills != null && jobSearchRequest.candidateSkills.Any())
            {
                foreach (var skill in jobSearchRequest.candidateSkills)
                {
                    shouldQueries.Add(new MatchQuery(Infer.Field<JobPostingDocument>(f => f.Requirement))
                    {
                        Query = skill
                    });
                }
            }

            int minLevel = Math.Max(0, jobSearchRequest.candidateLevel - 1);
            var mustQueries = new List<Query>
    {
       
        new TermQuery(Infer.Field<JobPostingDocument>(f => f.IsActive)) { Value = true },
        new NumberRangeQuery(Infer.Field<JobPostingDocument>(f => f.YearsOfExperience))
        {
            Gte = minLevel,
            Lte = jobSearchRequest.candidateLevel
        }
    };

            if (!string.IsNullOrEmpty(jobSearchRequest.candidateLocation))
            {
                mustQueries.Add(new MatchQuery(Infer.Field<JobPostingDocument>(f => f.Location))
                {
                    Query = jobSearchRequest.candidateLocation
                });
            }

            var boolQuery = new BoolQuery
            {
                Must = mustQueries,
                Should = shouldQueries
            };

            if (shouldQueries.Any())
            {
                boolQuery.MinimumShouldMatch = 1;
            }


            var searchRequest = new SearchRequest<JobPostingDocument>("job_postings")
            {
                Size = 50,
                Query = boolQuery 
            };

            var response = await _elasticSearchClient.SearchAsync<JobPostingDocument>(searchRequest);

            if (!response.IsValidResponse)
            {
                Console.WriteLine(response.DebugInformation);
                return new List<JobPostingDocument>();
            }

            return response.Documents.ToList();
        }
    }
}
