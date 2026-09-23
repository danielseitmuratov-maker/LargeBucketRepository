using System;

namespace _Root._Scripts.Infrastructure.Services.Input
{
    public interface IAttackButton
    {
        event Action Performed;
    }
}