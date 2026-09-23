public interface IUpgrade
{
    string Id { get; }              // "cart_speed", "income_multiplier"
    string DisplayName { get; }     // локализуемый ключ/название
    int Level { get; }              // текущий уровень
    int MaxLevel { get; }
    
    bool CanLevelUp(int availableCurrency);
    int GetPriceForNextLevel();
    
    void Apply();                   // применить эффект (Level уже увеличен)
    void Revert();                  // если надо уметь откатывать
    void LoadState(int savedLevel); // для восстановления прогресса
}