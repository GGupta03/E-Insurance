using E_Insurance_App.Models.Entities;

namespace E_Insurance_App.Services.Implementations
{
    public class PremiumService
    {
        public decimal CalculatePremium(
            PolicyType policy,
            int customerAge)
        {
            decimal basePremium = policy.BasePremium;

            decimal interestComponent =
                basePremium *
                (policy.InterestRate / 100) *
                policy.TermYears;

            int ageDiff =
                Math.Max(customerAge - policy.MinAge, 0);

            decimal ageRisk =
                ageDiff * 50;

            return basePremium +
                   interestComponent +
                   ageRisk;
        }
    }
}
