using KeepGrouped.API;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Storage;

namespace KeepGrouped.API.Users;

public class PatchAvatarCommand(UploadFileCommand uploadFileCmd, DeleteFileCommand deleteFileCmd, KeepGroupedDb db) : IHandler
{
    private readonly UploadFileCommand _uploadFileCmd = uploadFileCmd;
    private readonly DeleteFileCommand _deleteFileCmd = deleteFileCmd;
    private readonly KeepGroupedDb _db = db;

    public async Task<Result<GetFileMetaResponse>> ExecuteAsync(IFormFile file, User user)
    {
        if (!file.ContentType.StartsWith("image"))
        {
            return UserProblems.InvalidAvatarType(file.ContentType);
        }
        if (user.Avatar is not null)
        {
            await _deleteFileCmd.ExecuteAsync(user.Avatar.Key);
        }
        var uploadedFile = await _uploadFileCmd.ExecuteAsync(file.OpenReadStream(), user.Id + "_avatar", file.ContentType, file.Length, user);

        if (uploadedFile.IsProblem)
        {
            return uploadedFile.Problem;
        }

        user.Avatar = uploadedFile.Value;
        await _db.SaveChangesAsync();
        return GetFileMetaResponse.FromEntity(user.Avatar);
    }
}
