

var ddr = $("#CurrWxUserId").val();
if (ddr == null || ddr == "") {
    $("#wxnamenull").click();
}


var $wfitems = "";

function LoadMainFormData(currId) {

    var $url = woowx.constant.APIBaseURL + "/api/company/KhView";
    $.ajax({
        type: 'GET',
        url: $url,
        data:
        {
            Id: currId
        },
        dataType: 'json',
        timeout: 10000,
        success: function (data) {
            var $data = JSON.parse(data);
            $wfitems = $data.Data.WfItem;
            if ($data.Data != null && $data.Data != undefined) {

                $.each($data.Data, function (key, value) {
                    $("#" + key).val(value);
                });

            }
        }, error: function (xhr, type) {

            alert('LoadMainFormData系统异常' + xhr + ":" + type + ":" + xhr.status);
        }
    });


}
//下载文件
function downloadtxt(txtId) {

    window.open(woowx.constant.APIBaseURL + "/api/company/DownLoadFile?txtId=" + txtId + "&loadtype=1");

}

//删除服务记录
function delcustomerfw(custId) {

    var $url = woowx.constant.APIBaseURL + "/api/company/DeleteFwRows";
    $.ajax({
        type: 'GET',
        url: $url,
        data:
        {
            Id: custId
        },
        dataType: 'json',
        timeout: 10000,
        success: function (data) {
            var $data = JSON.parse(data);
            if ($data.Code === 0) {

                window.location.href = "/Common/SuccMag";

            } else {
                window.location.href = "/Common/FailMsg";
            }

        }, error: function (xhr, type) {

            alert('LoadMainFormData系统异常' + xhr + ":" + type + ":" + xhr.status);
        }
    });


}
/***合同文本**/
function ShowContText(currId) {

    var $urls = woowx.constant.APIBaseURL + "/api/company/GetcompViwe";
    $.ajax({///GetCountViwe
        type: 'Get',
        url: $urls,
        data:
        {
            // UserId: 1,
            Id: currId
        },

        dataType: 'json', timeout: 6000,
        success: function (data) {
            var $data = JSON.parse(data);
            var length = $data.Data.length;
            var resultstr2 = "";
            if (length > 0) {

                for (var i = 0; i < length; i++) {

                    resultstr2 += '<div class="weui-form-preview weui-panel_access">'
                        + '<div class="weui-form-preview__hd">'
                        + '<label class="weui-form-preview__label">服务事项</label>'
                        + '<em class="weui-form-preview__value">' + $data.Data[i].CategoryName + '</em>'
                        + '</div >'
                        + '<div class="weui-form-preview__hd">'
                        + '<label class="weui-form-preview__label">提醒时间</label>'
                        + '<span class="weui-form-preview__value">' + $data.Data[i].TxDate + '</span>'
                        + '</div>'
                        + '<div class="weui-form-preview__bd">'

                        + '<div class="weui-form-preview__item">'
                        + '<label class="weui-form-preview__label">服务描述</label>'
                        + '<span class="weui-form-preview__value">'
                        + $data.Data[i].Remark
                        + '</span>'
                        + '</div>'
                        + '</div>'

                   // debugger;
                    var pichtml = "";
                    var pics = $data.Data[i].PicData;
                    if (pics != null && pics.length > 0) {
                        pichtml +='<div class="weui-form-preview__bd">'
                        pichtml+= '<div class="weui-form-preview__item">'
                        pichtml+= '<ul class="weui-uploader__files" id="uploaderFiles">'
                        //console.log(resultstr2);
                        for (var j = 0; j < pics.length; j++) {
                            pichtml+='<li class="weui-uploader__file" style="background-image:url(' + pics[j].PicPath + ')"></li>'
                        }

                        pichtml+='</ul>'
                        pichtml+= '</div>'
                        pichtml+= '</div>'
                    }
                    if (pichtml != "") {
                        //console.log("pichtml==>"+pichtml);
                        resultstr2 += pichtml;
                    }

                    //+'<li class="weui-uploader__file" style="background-image:url(/Uploads/CustomerFile/3ce02b38-3e8c-4236-9a81-cdd46d641b94.jpeg)"></li>'
                    //+'<li class="weui-uploader__file" style="background-image:url(/Uploads/CustomerFile/5f38ba75-3189-412e-8d1a-2500fe78fa85.jpeg)"></li>'


                    resultstr2 += '<div class="weui-form-preview__ft" >'
                    resultstr2 += '<a class="weui-form-preview__btn weui-form-preview__btn_default" style="color:#FA5151" onclick = delcustomerfw(' + $data.Data[i].Id +')  href = "javascript:" ><i class="icon icon-115"></i> 删除</a>'
                   /*  resultstr2 += '<a class="weui-form-preview__btn weui-form-preview__btn_primary" onclick=delcustomerfw('+$data.Data[i].Id +')  href="javascript:"><i class="icon icon-115"></i>下载图片</a>'*/
                    resultstr2 += '</div>'
                    resultstr2 += '</div>'

                    //if (i == 0) {
                    //    resultstr2 += '<div class="page">'
                    //        + '<div class="page__bd">'
                    //        //+ '<a class="weui-cell " style="color:#000"  href="javascript:">'
                    //        //+ '<div class="weui-cell__bd"><p>服务事项</p></div>'
                    //        //+ '<div class="weui-cell__ft">' + $data.Data[i].CategoryName + '</div>'
                    //        //+ '</a>'
                    //        + '<div class="weui-cell"  style="color:#000" href="javascript:">'
                    //        + '<div class="weui-cell__bd"><p>服务描述</p></div>'
                    //       // + '<div class="weui-cell__ft"><span>' + $data.Data[i].Remark+ '</span></div >'
                    //        + '<div class="weui-cell__ft"><textarea class="weui-textarea"  rows="4">' + $data.Data[i].Remark + '</textarea></div >'
                    //        + '</div>'

                    //        + '<div class="weui-cell " style="color:#000"  href="javascript:">'
                    //        + '<div class="weui-cell__bd"><p>提醒时间</p></div>'
                    //        + '<div class="weui-cell__ft">' + $data.Data[i].TxDate + '</div>'
                    //        + '</div>'


                    //        + '<div class="weui-cell  " >'
                    //        + '<div class="weui-cell__bd">'
                    //        + '<a href="javascript:;" onclick=delcustomerfw(' + $data.Data[i].Id + ') class="weui-btn weui-btn_mini bg-red"><i class="icon icon-115"></i>删除</a>'
                    //        + '</div>'
                    //        + '<div class="weui-cell__ft">'
                    //        + ' <a href="javascript:;" onclick=downloadtxt(' + $data.Data[i].Id + ')  class="weui-btn weui-btn_mini bg-blue"><i class="icon icon-115"></i>下载图片</a>'
                    //        + '</div>'
                    //        + '</div>'


                    //       // + '<a href="javascript:;" onclick=downloadtxt(' + $data.Data[i].Id + ') class="weui-btn weui-btn_primary">下载图片</a>'
                    //        + '</div>'
                    //        + '</div>'
                    //} else {
                    //    //margin-top:设置条数之间间隔
                    //    resultstr2 += '<div class="page">'
                    //        + '<div class="page__bd">'
                    //        //+ '<a class="weui-cell " style="color:#000"  href="javascript:">'
                    //        //+ '<div class="weui-cell__bd"><p>服务事项</p></div>'
                    //        //+ '<div class="weui-cell__ft">' + $data.Data[i].CategoryName + '</div>'
                    //        //+ '</a>'
                    //        + '<div class="weui-cell" style="color:#000" href="javascript:">'
                    //        + '<div class="weui-cell__bd"><p>服务描述</p></div>'
                    //    /* + '<div class="weui-cell__ft"><span>' + $data.Data[i].Remark + '</span></div >'*/
                    //        + '<div class="weui-cell__ft"><textarea class="weui-textarea"  rows="4">' + $data.Data[i].Remark + '</textarea></div >'
                    //        + '</div>'
                    //        + '<div class="weui-cell " style="color:#000"  href="javascript:">'
                    //        + '<div class="weui-cell__bd"><p>提醒时间</p></div>'
                    //        + '<div class="weui-cell__ft">' + $data.Data[i].TxDate + '</div>'
                    //        + '</div>'
                    //        //+ '<a href="' + downloadurl + '" class="weui-btn weui-btn_primary">下载</a>'
                    //       // + '<a href="javascript:;" onclick=downloadtxt(' + $data.Data[i].Id + ') class="weui-btn weui-btn_primary">下载图片</a>'
                    //        + '<div class="weui-cell  " >'
                    //        + '<div class="weui-cell__bd">'
                    //        + '<a href="javascript:;" onclick=delcustomerfw(' + $data.Data[i].Id + ') class="weui-btn weui-btn_mini bg-red"><i class="icon icon-115"></i>删除</a>'
                    //        + '</div>'
                    //        + '<div class="weui-cell__ft">'
                    //        + ' <a href="javascript:;" onclick=downloadtxt(' + $data.Data[i].Id + ')  class="weui-btn weui-btn_mini bg-blue"><i class="icon icon-115"></i>下载图片</a>'
                    //        + '</div>'
                    //        + '</div>'

                    //        + '</div>'
                    //        + '</div>'

                    //}
                }
            } else {
                resultstr2 += "<span class='f-red'>没有数据</span>";

            }
            //console.log(resultstr2);
            $("#ShowFile").html(resultstr2);

           
           
        }
    });
}





$(function () {

    var currId = $("#contId").val();//'@ViewData["contId"]'
    LoadMainFormData(currId);
    ShowContText(currId);




});

//8888888888888888图片打开最大化888888888888888888888888888888888888888888888888888888
//让图片可以点击
function showload() {
    var tmpl = '<li class="weui-uploader__file" style="background-image:url(#url#)"></li>';
    var $uploaderInput = $("#uploaderInput"); //上传按钮+
    var $uploaderFiles = $("#uploaderFiles");    //图片列表
    var $galleryImg = $(".weui-gallery__img");//相册图片地址
    var $gallery = $(".weui-gallery");
    $uploaderInput.on("change", function (e) {
        var src, url = window.URL || window.webkitURL || window.mozURL, files = e.target.files;
        for (var i = 0, len = files.length; i < len; ++i) {
            var file = files[i];

            if (url) {
                src = url.createObjectURL(file);
            } else {
                src = e.target.result;
            }

            $uploaderFiles.append($(tmpl.replace('#url#', src)));
        }
    });
    $uploaderFiles.on("click", "li", function () {
        $galleryImg.attr("style", this.getAttribute("style"));
        console.log(this)
        $gallery.fadeIn(100);
    });
    $gallery.on("click", function () {
        $gallery.fadeOut(100);
    });

}
setTimeout(showload,2000)


//8888888888888888888888888888888888888888888888888888888888888888888888










