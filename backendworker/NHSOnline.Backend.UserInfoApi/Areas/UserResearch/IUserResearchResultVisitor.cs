namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch
{
    public interface IUserResearchResultVisitor<out T>
    {
        T Visit(PostUserResearchResult.Success result);
        T Visit(PostUserResearchResult.Failure result);
        T Visit(PostUserResearchResult.EmailMissing result);
        T Visit(PostUserResearchResult.InternalServerError result);
    }
}