namespace Backrooms.SceneAssembly.Contracts
{
    public interface ISceneAssembler
    {
        SceneAssemblyResult Assemble(SceneAssemblyPlan plan);
    }
}
