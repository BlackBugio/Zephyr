public class CharSkill
{
    private string SkillName { get; set; }
    private int SkillCost { get; set; }
    private int SkillLevel { get; set; }

    public CharSkill(string name, int cost, int level)
    {
        SkillName = name;
        SkillCost = cost;
        SkillLevel = level;
    }
}
