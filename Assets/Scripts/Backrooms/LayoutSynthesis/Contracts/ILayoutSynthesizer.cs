using Backrooms.LayoutSynthesis.Models;

namespace Backrooms.LayoutSynthesis.Contracts
{
    public interface ILayoutSynthesizer
    {
        LayoutSynthesisResult Synthesize(LayoutSynthesisRequest request);
    }
}
