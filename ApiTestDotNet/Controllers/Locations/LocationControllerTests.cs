using FluentAssertions;
using Flurl;
using Flurl.Http;
using IntegrationTestsTraining.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace IntegrationTestsTraining.Controllers.Locations
{
    public class LocationControllerTests
    {
        readonly String baseUrl = "https://rickandmortyapi.com/api/";
        readonly String controller = "location";

        [Fact]
        public async void GetAllLocationsReturnsStatusCodeOk()
        {
            var call = baseUrl.AppendPathSegments(controller);
            var response = await call.GetAsync();
            response.ResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void GetALocationByIdReturnsExpectedBody()
        {
            var locationId = 1;
            var call =  baseUrl.AppendPathSegments(controller, locationId);
            var response = await call.GetAsync();

            response.ResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = response.ResponseMessage.Content.ReadAsStringAsync().Result;
            var location = JsonConvert.DeserializeObject<Location>(content);
            location.Name.Should().Be("Earth (C-137)");
            location.Type.Should().Be("Planet");
        }

        [Fact]
        public async void GetAllLocationsReturnsExpectedCount()
        {
            var call = baseUrl.AppendPathSegment(controller);
            var jsonCountResults =0;
            string next=null;
            var infoCount = 0;
            var documentsPerPage = 20;
            String serializedResult = "";
            AllDataPerCategory deserializedResult;
            do
            {
                var response = await call.GetAsync();
                response.ResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

                serializedResult = response.ResponseMessage.Content.ReadAsStringAsync().Result;
                deserializedResult = JsonConvert.DeserializeObject<AllDataPerCategory>(serializedResult);
                deserializedResult?.Results.Count.Should().BeLessThanOrEqualTo(documentsPerPage);
                infoCount = deserializedResult.Info.Count;
                next = deserializedResult.Info.Next;
                jsonCountResults = deserializedResult.Results.Count+ jsonCountResults;

                
                call = next;
            } while (!String.IsNullOrEmpty(next));

            jsonCountResults.Should().Be(infoCount);
        }

        [Fact]
        public async void GetANonExistingLocationByIdReturnsNotFound()
        {
            var locationId = 1000;
            var call = baseUrl.AppendPathSegments(controller, locationId)
                .AllowAnyHttpStatus();
            var response = await call.GetAsync();

            response.ResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = response.ResponseMessage.Content.ReadAsStringAsync().Result;
            var errorM = JsonConvert.DeserializeObject<Errors>(errorMessage);
            errorM?.error.Should().Be("Location not found");
        }

    }
}
