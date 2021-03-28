using PetClinic.Data;
using System.Linq;

namespace PetClinic.DataProcessor
{
    public class Bonus
    {
        public static string UpdateVetProfession(PetClinicContext context, string phoneNumber, string newProfession)
        {
            var vet = context.Vets.FirstOrDefault(v => v.PhoneNumber == phoneNumber);

            if (vet is null)
            {
                return $"Vet with phone number {phoneNumber} not found!";
            }

            var oldProfession = vet.Profession;

            vet.Profession = newProfession;

            return $"{vet.Name}'s profession updated from {oldProfession} to {newProfession}.";
        }
    }
}