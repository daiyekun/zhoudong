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

    window.open(woowx.constant.APIBaseURL + "/api/company/DownLoadFile?txtId=" + txtId +"&loadtype=1");

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
                    //var downloadurl = woowx.constant.APIBaseURL + "/" + data[0].path;
                    if (i == 0) {
                        resultstr2 += '<div class="page">'
                            + '<div class="page__bd">'
                            //+ '<a class="weui-cell " style="color:#000"  href="javascript:">'
                            //+ '<div class="weui-cell__bd"><p>服务事项</p></div>'
                            //+ '<div class="weui-cell__ft">' + $data.Data[i].CategoryName + '</div>'
                            //+ '</a>'
                            + '<a class="weui-cell"  style="color:#000" href="javascript:">'
                            + '<div class="weui-cell__bd"><p>服务描述</p></div>'
                            + '<div class="weui-cell__ft"><span>' + $data.Data[i].Remark+ '</span></div >'
                            + '</a>'
                            + '<a class="weui-cell " style="color:#000"  href="javascript:">'
                            + '<div class="weui-cell__bd"><p>提醒时间</p></div>'
                            + '<div class="weui-cell__ft">' + $data.Data[i].TxDate + '</div>'
                            + '</a>'
                            + '<a href="javascript:;" onclick=downloadtxt(' + $data.Data[i].Id + ') class="weui-btn weui-btn_primary">下载图片</a>'
                            + '</div>'
                            + '</div>'
                    } else {
                        //margin-top:设置条数之间间隔
                        resultstr2 += '<div class="page">'
                            + '<div class="page__bd">'
                            //+ '<a class="weui-cell " style="color:#000"  href="javascript:">'
                            //+ '<div class="weui-cell__bd"><p>服务事项</p></div>'
                            //+ '<div class="weui-cell__ft">' + $data.Data[i].CategoryName + '</div>'
                            //+ '</a>'
                            + '<a class="weui-cell" style="color:#000" href="javascript:">'
                            + '<div class="weui-cell__bd"><p>服务描述</p></div>'
                            + '<div class="weui-cell__ft"><span>' + $data.Data[i].Remark + '</span></div >'
                            + '</a>'
                            + '<a class="weui-cell " style="color:#000"  href="javascript:">'
                            + '<div class="weui-cell__bd"><p>提醒时间</p></div>'
                            + '<div class="weui-cell__ft">' + $data.Data[i].TxDate + '</div>'
                            + '</a>'
                            //+ '<a href="' + downloadurl + '" class="weui-btn weui-btn_primary">下载</a>'
                            + '<a href="javascript:;" onclick=downloadtxt(' + $data.Data[i].Id + ') class="weui-btn weui-btn_primary">下载图片</a>'
                            + '</div>'
                            + '</div>'

                    }
                }
            } else {
                resultstr2 += "<span class='f-red'>没有数据</span>";

                   }

            $("#ShowFile").html(resultstr2);
        }
    });
}





$(function () {

    var currId = $("#contId").val();//'@ViewData["contId"]'
    LoadMainFormData(currId);
    ShowContText(currId);
    
    
    
   
});










