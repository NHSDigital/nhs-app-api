using System;
using System.Collections.Generic;
using System.Net;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class ReferenceDataBuilder
    {
        private readonly List<Entry<ReferenceDataResponse>> _sections = new List<Entry<ReferenceDataResponse>>();

        public ReferenceDataBuilder AddSection(string id, Action<CodingListBuilder> actions)
        {
            var codes = new CodingListBuilder();
            actions.Invoke(codes);

            _sections.Add(new Entry<ReferenceDataResponse>
            {
                Resource = new ReferenceDataResponse
                {
                    Id = id,
                    Concept = codes.Build()
                }
            });

            return this;
        }

        public OrganDonationResponse<ReferenceDataResponse> Build()
        {
            return new OrganDonationResponse<ReferenceDataResponse>(HttpStatusCode.OK)
            {
                Body = new OrganDonationSuccessResponse<ReferenceDataResponse>
                {
                    Entry = _sections
                }
            };
        }

        public class CodingListBuilder
        {
            private readonly List<Coding> _coding = new List<Coding>();

            public CodingListBuilder Add(string code, string display)
            {
                _coding.Add(new Coding { Code = code, Display = display });
                return this;
            }

            public List<Coding> Build()
            {
                return _coding;
            }
        }
    }
}