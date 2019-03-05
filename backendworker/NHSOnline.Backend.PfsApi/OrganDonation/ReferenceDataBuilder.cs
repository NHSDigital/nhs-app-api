using System;
using System.Collections.Generic;
using System.Net;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal class ReferenceDataBuilder
    {
        private readonly List<Entry<ReferenceData>> _sections = new List<Entry<ReferenceData>>();

        public ReferenceDataBuilder AddSection(string id, Action<CodingListBuilder> actions)
        {
            var codes = new CodingListBuilder();
            actions.Invoke(codes);

            _sections.Add(new Entry<ReferenceData>
            {
                Resource = new ReferenceData
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
                Body = new ReferenceDataResponse
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