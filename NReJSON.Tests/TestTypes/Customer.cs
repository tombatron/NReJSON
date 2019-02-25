using System;

namespace NReJSON.Tests.TestTypes
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime RegisteredOn { get; set; }

        public Address CorporateAddress { get; set; }

        public override bool Equals(object obj) =>
            this.GetHashCode() == obj.GetHashCode();

        public override int GetHashCode() =>
            new
            {
                Id,
                Name,
                RegisteredOn,
                CorporateAddress
            }.GetHashCode();
    }
}