using System;

namespace LightbulbPlayground
{
    public class ImmutableExample
    {
        public readonly string FirstName;
        public readonly string LastName;

        public ImmutableExample(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            }
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            }
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public ImmutableExample WithFirstName(string firstName)
        {
            return new ImmutableExample(firstName, this.LastName);
        }
    }

    public class Usage
    {
        public void SomeMethod()
        {
            var immutable = new ImmutableExample("Jan", "Kowalski");

            //immutable.FirstName = "Tomasz";
            //immutable.LastName = "Nowak";

            var newImmutable = immutable.WithFirstName("Tomasz");
        }
    }
}
