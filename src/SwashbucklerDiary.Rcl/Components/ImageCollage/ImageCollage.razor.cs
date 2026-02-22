using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ImageCollage : ComponentBase
    {
        [Parameter]
        [EditorRequired]
        public DiaryModel Diary { get; set; } = default!;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private List<string>? imagePaths;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            UpdateImagePaths();
        }

        private void UpdateImagePaths()
        {
            if (Diary == null || string.IsNullOrEmpty(Diary.Content))
            {
                imagePaths = null;
                return;
            }

            var resources = MediaResourceManager.GetDiaryResources(Diary.Content);
            var imageResources = resources.Where(r => r.ResourceType == MediaResource.Image).ToList();

            if (imageResources.Count == 0)
            {
                imagePaths = null;
                return;
            }

            imagePaths = new List<string>();
            foreach (var resource in imageResources)
            {
                var mediaResourcePath = MediaResourceManager.ToMediaResourcePath(NavigationManager, resource.ResourceUri);
                if (mediaResourcePath != null && !string.IsNullOrEmpty(mediaResourcePath.DisPlayedUrl))
                {
                    imagePaths.Add(mediaResourcePath.DisPlayedUrl);
                }
                else if (!string.IsNullOrEmpty(resource.ResourceUri))
                {
                    var linkBase = MediaResourceManager.MarkdownLinkBase;
                    if (!string.IsNullOrEmpty(linkBase))
                    {
                        imagePaths.Add($"{linkBase}/{resource.ResourceUri}");
                    }
                    else
                    {
                        imagePaths.Add(resource.ResourceUri);
                    }
                }
            }
            
            if (imagePaths.Count == 0)
            {
                imagePaths = null;
            }
        }
    }
}
