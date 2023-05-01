
//多图片上传
function removevideo(obj) {
    $.confirm('您确定要删除吗?', '确认删除?', function () {
        var delId = $(obj).children("input[name=files]").val();
        $.get("/NfCommon/WxFile/DeleteData", { Id: delId }, function (rs) {
            $(obj).remove();
        }, 'json');

    });
    return false;
}
function createVideoListItem(video) {
    const template = document.getElementById('videoListItemTemplate');
    const clone = template.content.cloneNode(true);

    const videoItem = clone.querySelector('video');
    videoItem.src = URL.createObjectURL(video);
    videoItem.poster = videoThumbnailURL; // 这里需要设置视频缩略图的 URL

    return clone;
}
//视频列表
function addVideoToList(video) {
    const videoList = document.getElementById('videoList');
    const videoListItem = createVideoListItem(video);
    videoList.appendChild(videoListItem);
}
//上传视频
function uploadvideo(obj) {
    debugger;
    var files = obj.files;
    var len = files.length;
    var $commpId = $("#CompanyId").val();//客户ID
    //for (var i = 0; i < len; i++) {
    //var fileInput = document.getElementById('uploaderInput');
    var file = files[0];
    var formData = new FormData();
    formData.append('video', file);

    // 使用 canvas 对视频进行压缩处理
    var video = document.createElement('video');
    var canvas = document.createElement('canvas');
    video.preload = 'metadata';
    //video.src = URL.createObjectURL(file);
    //video.currentTime = Math.min(1, video.duration);

    var ctx = canvas.getContext('2d');

    video.onloadedmetadata = function () {
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
        canvas.toBlob(function (blob) {
            formData.append('compressedVideo', blob, 'compressed.png');
            var thumbnailUrl = canvas.toDataURL('image/png');
            URL.revokeObjectURL(video.src);
            formData.append('imgbase64', 'upload.mp4');
            formData.append('commpId', $commpId);
            debugger;
            // 使用 jQuery 的 post 方法上传视频
            $.ajax({
                url: '/NfCommon/WxFile/WxUploadvideo',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    console.log(res)
                    //addVideoToList();
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        }, 'video/mp4', 0.8);
    };



}