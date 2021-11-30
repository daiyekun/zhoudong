var ddr = $("#WxCurrUserId").val();



if (ddr == null || ddr == "") {
    $("#wxnamenull").click();
}


function LoadMainFormData(currId) {
    var $url = woowx.constant.APIBaseURL + "/api/Contract/GetCountViwe";
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

            if ($data.Data != null && $data.Data != undefined) {

                $.each($data.Data, function (key, value) {
                    $("#" + key).val(value);
                });
                CheckWflow(currId);
                GetOptions(currId);//意见
            }
        }, error: function (xhr, type) {
            alert('LoadMainFormData系统异常' + xhr + ":" + type + ":" + xhr.status);
        }
    });


}
//下载文件
function downloadtxt(txtId)
{
    var ext = $("#ext_" + txtId).val();
   // alert(ext);

    if (ext.indexOf("pdf") >= 0 || ext.indexOf("PDF") >= 0) {

        var fileurl = '/js/nf-plugs/pdfjs/web/viewer.html?file=' +
            encodeURIComponent(woowx.constant.APIBaseURL + '/api/Contract/GetPdf?Id=' + txtId);
        window.open(fileurl);
    } else {
        window.open(woowx.constant.APIBaseURL + "/Contract/DownLoadTxt?txtId=" + txtId);

    }
   


}
/***合同文本**/
function ShowContText(currId) {
    var $urls = woowx.constant.APIBaseURL + "/api/Contract/GetContTextViwe";
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

            var length = data.length;
            var resultstr2 = "";
            if (length > 0) {
               
                for (var i = 0; i < length; i++) {
                    //var downloadurl = woowx.constant.APIBaseURL + "/" + data[0].path;
                    if (i == 0) {
                        resultstr2 += '<div class="page">'
                            + '<div class="page__bd">'
                            + '<a class="weui-cell"  style="color:#000" href="javascript:">'
                            + '<div class="weui-cell__bd"><p>文件名称</p></div>'
                            + '<div class="weui-cell__ft"><span>' + data[i].name + '</span></div >'
                            + '</a>'
                            + '<a class="weui-cell " style="color:#000"  href="javascript:">'
                            + '<div class="weui-cell__bd"><p>文件类别</p></div>'
                            + '<div class="weui-cell__ft">' + data[i].contTxtType + '</div>'
                            + '<input type="hidden" id="ext_' + data[i].id + '" value="' + data[i].extenName+'"/>'
                            + '</a>'
                            + '<a href="javascript:;" onclick=downloadtxt('+data[i].id+') class="weui-btn weui-btn_primary">查看</a>'
                            + '</div>'
                            + '</div>'
                    } else {
                        //margin-top:设置条数之间间隔
                        resultstr2 += '<div class="page">'
                            + '<div class="page__bd">'
                            + '<a class="weui-cell" style="color:#000" href="javascript:">'
                            + '<div class="weui-cell__bd"><p>文件名称</p></div>'
                            + '<div class="weui-cell__ft"><span>' + data[i].name + '</span></div >'
                            + '</a>'
                            + '<a class="weui-cell " style="color:#000"  href="javascript:">'
                            + '<div class="weui-cell__bd"><p>文件类别</p></div>'
                            + '<div class="weui-cell__ft">' + data[i].contTxtType + '</div>'
                            + '<input type="hidden" id="ext_' + data[i].id + '" value="' + data[i].extenName+'"/>'
                            + '</a>'
                            //+ '<a href="' + downloadurl + '" class="weui-btn weui-btn_primary">下载</a>'
                            + '<a href="javascript:;" onclick=downloadtxt(' + data[i].id + ') class="weui-btn weui-btn_primary">查看</a>'
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
//判断是否可以审批
function CheckWflow(currId) {
    var $userId = $("#CurrWxUserId").val();//当前登录的用户
    var $wfitem = $("#WfItem").val();//审批事项
    var $url = woowx.constant.APIBaseURL + "/api/WorkFlow/GetFlowPermission";
    var flowPerm = {
        SpId: currId,
        WfItem: $wfitem,
        UserId: $userId,
        SpType: 3//合同
    };
    var poststr = JSON.stringify(flowPerm);
   
    $.ajax({
        type: 'POST',
        url: $url,
        data: poststr,
        dataType: 'json',
        contentType: 'application/json;charset=UTF-8',
        timeout: 10000,
        success: function (data) {
            var $data = JSON.parse(data);
            var redata = $data.Data;
            if (redata != null && redata != undefined) {

                $("#InstId").val(redata.InstId);
                $("#NodeId").val(redata.NodeId);
                $("#NodeStrId").val(redata.NodeStrId);
                if (redata.Qx === 1) {
                    $("#shenpi").removeClass("hide").addClass("show");
                }


            }
        }, error: function (xhr, type) {
            alert('CheckWflow系统异常' + xhr + ":" + type + ":" + xhr.status);
        }
    });


}
//提交审批意见
function SubmitOption(currId, OptRes, opmsg) {
    var $userId = $("#CurrWxUserId").val();//当前登录的用户
    var $wfitem = $("#WfItem").val();//审批事项
    var $url = woowx.constant.APIBaseURL + "/api/WorkFlow/SubmitAgreeOption";
    if (OptRes === 2) {//不同意
        $url = woowx.constant.APIBaseURL + "/api/WorkFlow/SubmitDisagreeOption";

    }
    var otion = {
        InstId: $("#InstId").val(),//审批实例ID
        ObjType: 3,
        ObjMoney: $("#HtJeD").val(),//合同金额
        SpType: 3,//合同
        ObjId: currId,
        Option: opmsg,
        OptRes: OptRes,
        SubmitWxId: $userId
    };
    var poststr = JSON.stringify(otion);
   
    $.ajax({
        type: 'POST',
        url: $url,
        data: poststr,
        dataType: 'json',
        contentType: 'application/json;charset=UTF-8',
        timeout: 10000,
        success: function (data) {
            var $data = JSON.parse(data);
            if ($data.Code === 0) {
                window.location.href = "/Common/SuccMag";
            } else {
                window.location.href = "/Common/FailMsg";
            }
        }, error: function (xhr, type) {
            var errmsg = "错误类型：" + type + ",xhr.status:" + xhr.status
            window.location.href = "/Common/FailMsg?errmsg=" + errmsg;
          
        }
    });


}
//获取审批情况
function GetOptions(currId) {
    var $userId = $("#CurrWxUserId").val();//当前登录的用户

    var $wfitem = $("#WfItem").val();//审批事项
    var $url = woowx.constant.APIBaseURL + "/api/WorkFlow/GetWooFlowTime";
    var flowPerm = {
        SpId: currId,
        WfItem: $wfitem,
        UserId: $userId,
        SpType: 3//合同
    };
   
    var poststr = JSON.stringify(flowPerm);

    $.ajax({
        type: 'POST',
        url: $url,
        data: poststr,
        dataType: 'json',
        contentType: 'application/json;charset=UTF-8',
        timeout: 10000,
        success: function (data) {
            var $data = JSON.parse(data);
            var redata = $data.Data;
            var instzt = 0;//审批实例状态
            var length = redata.length;
            if (redata != null && redata != undefined && length > 0) {
                var timestr = '<ul>';
                var titleclass = "";
                for (var i = 0; i < length; i++) {
                    titleclass = redata[i].Tstate > 0 ? "recent" : "";
                    if (redata[i].Name == "结束") {
                        timestr += '<li class="timeline-item">'
                        if (redata[i].Tstate > 0) {
                            timestr += '<div class="timeline-item-color timeline-item-head-first">'
                            timestr += '<i class="timeline-item-checked   weui-icon-success-no-circle"></i>'
                            timestr += '</div>'

                        } else {
                            timestr += '<div class="timeline-item-color timeline-item-head"></div>'
                        }
                        timestr += '<div class="timeline-item-tail hide"></div>'
                        timestr += '<div class="timeline-item-content">'
                        var ottime = (redata[i].Options[0] != null && redata[i].Options[0].OpTime!=null) ? redata[i].Options[0].OpTime : ""
                        timestr += '<h4 class="' + titleclass + '">【' + redata[i].Name + '】&nbsp;' + ottime+ '</h4>'
                        timestr += '</div>'
                        timestr += '</li>'
                    } else {
                        timestr += '<li class="timeline-item">'
                        if (redata[i].Tstate > 0) {
                            timestr += '<div class="timeline-item-color timeline-item-head-first">'
                            timestr += '<i class="timeline-item-checked   weui-icon-success-no-circle"></i>'
                            timestr += '</div>'

                        } else {
                            timestr += '<div class="timeline-item-color timeline-item-head"></div>'
                        }


                        timestr += '<div class="timeline-item-tail"></div>'
                        timestr += '<div class="timeline-item-content">'
                        timestr += '<h4 class="' + titleclass + '">【' + redata[i].Name + '】</h4>'
                        var options = redata[i].Options;
                        for (var j = 0; j < options.length; j++) {
                            timestr += '<div class="' + titleclass + '">'
                            //timestr += '<div>' + options[j].YuJian + '<div>'
                            timestr += '<div><span>' + options[j].UserName + ':&nbsp;&nbsp;</sapn><span>' + options[j].YuJian + '</span><div>'
                             timestr += '<div>' + options[j].OpTime +'<div>'
                            timestr += '</div >';

                        }


                        timestr += '</div>'
                        timestr += '</li>'
                    }

                }


                timestr += '</ul>';


                $('#timelineui').append(timestr);



            } else {
                var resultstr2 = "<span class='f-red'>没有数据</span>";
                $('#timelineui').append(resultstr2);
            }
        }, error: function (xhr, type) {
            alert('CheckWflow系统异常' + xhr + ":" + type + ":" + xhr.status);
        }
    });
}


$(function () {

    var currId = $("#contId").val();//'@ViewData["contId"]'
    LoadMainFormData(currId);
    ShowContText(currId);
    // CheckWflow(currId);
    //审批弹框
    $(document).on("click", "#shenpi", function () {
        $.modal({
            title: "<span class='f-orange'>审批意见</span>",
            text: "<textarea class='weui-form-area'  name='wfoption' id='wfoption' rows='3' cols=20' placeholder='意见必填'></textarea>",
            buttons: [
                {
                    text: "同意", onClick: function () {
                        var wfoption = $("#wfoption").val();
                        if (wfoption !== "" && wfoption !== undefined) {
                            SubmitOption(currId, 1, wfoption);
                        } else {
                            $.toptip('请输入意见', 'warning')
                        }

                    }
                },
                {
                    text: "<span class='f-red'>不同意</span>", onClick: function () {
                        var wfoption = $("#wfoption").val();
                        if (wfoption !== "" && wfoption !== undefined) {
                            SubmitOption(currId, 2, wfoption);
                        } else {
                            $.toptip('请输入意见', 'warning')
                        }


                    }
                },
                { text: "取消", className: "default", onClick: function () { } },
            ]
        });
    });
    //审批图标可以移动
    moveEvent("shenpi");
});





