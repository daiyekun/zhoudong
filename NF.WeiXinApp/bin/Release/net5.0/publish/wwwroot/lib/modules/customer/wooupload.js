// 根据文件大小转换成合适的单位
function get_readable_size(size) {
    var units = ['B', 'KB', 'MB', 'GB'];
    var i;
    for (i = 0; size >= 1024 && i < 4; i++) {
        size /= 1024;
    }
    return size.toFixed(2) + units[i];
}

// 将 bytes 转换成 MB
function bytesToMB(bytes) {
    return (bytes / (1024 * 1024)).toFixed(2) + 'MB';
}



//function generateThumb(video, index) {
//    var canvas = document.createElement('canvas');
//    canvas.width = 120;
//    canvas.height = 90;
//    var ctx = canvas.getContext('2d');
//    video.onloadedmetadata = function () { // 加载视频信息
//        debugger;
//        //playVideo(video.src, index);
//        ctx.clearRect(0, 0, canvas.width, canvas.height); // 清除画布
//        ctx.beginPath();
//        ctx.drawImage(video, 0, 0, 120, 90);

//        var img = new Image();
//        img.onloadeddata= function () { // 重新构建图片
//            URL.revokeObjectURL(this.src); // 清楚内存
//            $(canvas).remove(); // 删除绘制的canvas
//        }
//        img.src = canvas.toDataURL();
//        img.onclick = function () {
//            playVideo(video.src, index);
//        }
//        var $fileList = $('#vdieoList');
//        var $total = $('#total');
//        $fileList.append($('<li class="weui-uploader__file"></li>').append(img));
//        totalFiles++;
//        $total.html(totalFiles);
//        return img;
//    };
//}

//function generateThumb(video, index) {
//    var canvas = document.createElement('canvas');
//    canvas.width = 120;
//    canvas.height = 90;
//    var ctx = canvas.getContext('2d');
//    ctx.fillStyle = "#FFFFFF";
//    // 将 onloadedmetadata 绑定到 video 对象上
//    video.onloadedmetadata = function () { // 加载视频信息
//        debugger;
//        //playVideo(video.src, index);
//        ctx.clearRect(0, 0, canvas.width, canvas.height); // 清除画布
//        ctx.beginPath();
//        ctx.drawImage(video, 0, 0, 120, 90);

//        var img = new Image();
//        img.onload = function () { // 重新构建图片
//            URL.revokeObjectURL(this.src); // 清楚内存
//            $(canvas).remove(); // 删除绘制的canvas
//        }

//        // 将 toDataURL 放到 onload 里执行，确保视频已经加载完成
//        video.onloadeddata = function () {
//            img.src = canvas.toDataURL();

//            // 点击图片播放对应的视频
//            img.onclick = function () {
//                playVideo(video.src, index);
//            }
//            var $fileList = $('#vdieoList');
//            var $total = $('#total');
//            $fileList.append($('<li class="weui-uploader__file"></li>').append(img));
//            totalFiles++;
//            $total.html(totalFiles);
//            return img;
//        };
//    };
//}

function generateThumb(video, index) {
    var canvas = document.createElement('canvas');
    canvas.width = 120;
    canvas.height = 90;
    var ctx = canvas.getContext('2d');

    // 加载视频信息并在完成时执行回调函数
    video.onloadedmetadata = function () {
        video.onloadeddata = function () {

            // 清空画布
            ctx.clearRect(0, 0, canvas.width, canvas.height);

            // 绘制视频帧
            ctx.drawImage(video, 0, 0, 120, 90);

            // 创建新图像对象并将其显示在页面上
            var img = new Image();
           /* img.crossOrigin = 'Anonymous';*/
            img.onload = function () {
                URL.revokeObjectURL(this.src);
                $(canvas).remove();

                // 显示图像并添加单击事件将视频播放
                img.onclick = function () {
                    playVideo(video.src, index);
                };
                var $fileList = $('#vdieoList');
                var $total = $('#total');
                $fileList.append($('<li class="weui-uploader__file"></li>').append(img));
                totalFiles++;
                $total.html(totalFiles);
            }

            // 使用toDataURL()方法获取绘制的图像数据
            img.src = canvas.toDataURL();
        };
    };

    // 视频加载失败时进行处理
    video.onerror = function (err) {
        console.log('Video error', err);
    };

}



// 播放视频
function playVideo(src, index) {
    //var player = videojs('player', {
    var player = videojs(document.getElementById('myVideo'), {
        controls: true,
        autoplay: true,
        preload: 'auto'
    });
    player.src({ type: 'video/mp4', src: src });
    $('#modal-video').show();
}

// 隐藏 modal
function hideModal() {
    $('#modal-video').hide();
}

// 上传队列
var uploadQueue = [];

// 文件总数，最多支持上传10个文件
var totalFiles = 0;

$(function () {
    // 上传区域
    var $uploaderInput = $('#uploadervideoInput');
    var $fileList = $('#vdieoList');
    var $total = $('#total');

    // 选择文件后触发
    $uploaderInput.on('change', function (event) {
        debugger;
        var files = event.target.files;
        $.each(files, function (index, file) {
            debugger;
            // 如果已经上传了10个视频
            if (totalFiles == 9) {
                weui.alert("最多上传9个视频");
                return false;
            }

            // 添加到队列中
            uploadQueue.push(file);

            // 生成缩略图
            var video = document.createElement('video');
            video.src = URL.createObjectURL(file);
            video.id = "player";
            video.addEventListener('loadeddata', function () {
                var img = generateThumb(video, totalFiles);
                //var img = generateThumb(video);
                //document.body.appendChild(img);
                //URL.revokeObjectURL(video.src); // 释放 blob URL
            });
            video.onloadedmetadata = function () {
                URL.revokeObjectURL(this.src);
            }
            var img =  generateThumb(video, totalFiles);
            //$fileList.append($('<li class="weui-uploader__file"></li>').append(img));
            //totalFiles++;
            //$total.html(totalFiles);
        });
    });

    // 提交表单
    $('#btnSubmit').on('click', function () {
        if (uploadQueue.length == 0) {
            weui.alert('请选择要上传的视频');
            return;
        }

        // 显示预处理提示框
        weui.showLoading('正在处理');

        // 遍历队列，一个一个上传
        $.each(uploadQueue, function (index, file) {
            var formData = new FormData();
            formData.append('video', file, file.name);
            // 发送请求上传
            $.ajax({
                url: '/api/v1/upload',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (result) {
                    // 上传成功，生成视频播放链接
                    var videoUrl = '/api/v1/videos/' + result.fileName;
                    var thumbUrl = '/api/v1/thumbnails/' + result.fileName;
                    var html = '<div class="weui-grid js_grid" style="width: 30%;"><a href="' + videoUrl + '" style="display:block"><img class="thumbnail" src="' + thumbUrl + '"></a></div>';
                    $('#videoList').append(html);

                    // 发送请求保存文件信息到数据库
                    $.ajax({
                        url: '/api/v1/videos',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify({
                            name: result.fileName,
                            size: bytesToMB(result.fileSize),
                            thumbnail: thumbUrl,
                            url: videoUrl
                        }),
                        success: function () {
                            // 保存成功
                        }
                    });

                    // 如果是最后一个文件上传成功
                    if ((index + 1) == uploadQueue.length) {
                        // 隐藏预处理提示框
                        weui.hideLoading();
                        // 提示上传完成
                        weui.toast('上传完成');
                    }
                }
            });
        });

        // 清空队列
        uploadQueue = [];
        totalFiles = 0;
        $fileList.empty();
        $total.html(totalFiles);
    });
});