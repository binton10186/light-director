namespace LightDirector.Domain
{
   public class CueResult
   {
      public CueResult(ParCanSetting[] parCanSettings, bool shouldMoveToNext)
      {
         ParCanSettings = parCanSettings;
         ShouldMoveToNext = shouldMoveToNext;
      }

      public ParCanSetting[] ParCanSettings { get; private set; }
      public bool ShouldMoveToNext { get; private set; }
   }
}