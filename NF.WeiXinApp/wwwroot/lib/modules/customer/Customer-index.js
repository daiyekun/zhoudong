var ddr = $("#Wx").val();
if (ddr == null || ddr == "") {
    $("#wxnamenull").click();
}

var $url = woowx.constant.APIBaseURL + "/api/Company/WooKhlist";
var pagesize = woowx.constant.page;//每页数据条数
var page = 1;
var maxpage;
var HtType = $("#HtType").val();
var Wxz =$('#Wx').val();
var flag = true;
var $kekword = "";

/**
 * 分页
 * @param {any} spage 开始页
 */
function ajaxpage1(spage) {
    // alert("page:" + spage + "，limit=" + pagesize + ",keyWord=" + $kekword + ",Wxzh=" + Wxz + ",FinanceType=" + HtType);
    $.ajax({
        type: "GET",
        url: $url,
        data: { "page": page, "limit": pagesize, "keyWord": $kekword, "Wxzh": Wxz, "FinanceType": HtType },
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        timeout: 16000,
        beforeSend: function (xhr) {
            $('#loading').show();
        },
        success: function (rs) {
            var datas = JSON.parse(rs);
            $('#loading').hide();
            if (datas.data == null) {
                $("#getmore").html("没有更多数据了");
                return false
            }
            if (datas.data.length < pagesize) {
                flag = false//设置没有数据了标记
                $("#getmore").html("没有更多数据了");
            }
            var result = '';
            for (var i = 0; i < datas.data.length; i++) {
                result += '<div class="weui-cells">'
                    + '<a class="weui-cell  weui-cell_access" href="Detail?Id=' + datas.data[i].Id + "&FinanceType=" + HtType + '">'
                    + '<div class="weui-cell__bd">'
                    + ' <p>名称</p>'
                    + '</div>'
                    + '<div class="weui-cell__ft list-title">' + datas.data[i].Name + '</div>'
                    + '</a>'
                    //+ '<div class="weui-cell  " >'
                    //+ '<div class="weui-cell__bd">'
                    //+ '<p>编号</p>'
                    //+ '</div>'
                    //+ '<div class="weui-cell__ft">' + datas.data[i].Code + '</div>'
                    //+ '</div>'
                    //+ '<div class="weui-cell  " >'
                    //+ '<div class="weui-cell__bd">'
                    //+ '<p>联系人</p>'
                    //+ '</div>'
                    //+ '<div class="weui-cell__ft">' + datas.data[i].FirstContact + '</div>'
                    //+ '</div>'
                    //+ '<div class="weui-cell  " >'
                    //+ '<div class="weui-cell__bd">'
                    //+ '<p>联系电话</p>'
                    //+ '</div>'
                    //+ '<div class="weui-cell__ft">' + datas.data[i].FirstContactMobile + '</div>'
                    //+ '</div>'

                    + '<div class="weui-cell  " >'
                    + '<div class="weui-cell__bd">'
                    + ' <a href="javascript:;" onclick=upcustomer(' + datas.data[i].Id + ') class="weui-btn weui-btn_mini bg-green"><i class="icon icon-115"></i>修改</a>'
                    + ' &nbsp;&nbsp;<a href="javascript:;" onclick=delcustomer(' + datas.data[i].Id + ') class="weui-btn weui-btn_mini bg-red"><i class="icon icon-115"></i>删除</a>'
                    + '</div>'
                    + '<div class="weui-cell__ft">'
                    + ' <a href="javascript:;" onclick=addfuwu(' + datas.data[i].Id + ')  class="weui-btn weui-btn_mini bg-blue"><i class="icon icon-115"></i>新增服务记录</a>'
                    + '</div>'
                    + '</div>'
                    + '</div>'

            }
            $("#rank-list").append(result);


        },
        error: function (xhr) {
            alert('ajax出错');
        },
    });
}
//初始化数据从第一页数据开始请求
ajaxpage1(page);
//更多数据
$('#getmore').on('click', function () {
    if (flag) {
        page = page + 1;//如果可以就是页码加1
        ajaxpage1(page);
    }
});
//回车搜索
$('#searchInput').bind('keypress', function (event) {
    if (event.keyCode == "13") {
        $("#rank-list").html("");
        $kekword = $('#searchInput').val();
        page = 1;
        ajaxpage1(page);
    }
});
//搜索按钮
$("#btnsearch").on("click", function () {
    $("#rank-list").html("");
    $kekword = $('#searchInput').val();
    page = 1;
    ajaxpage1(page);
})
//修改用户
function upcustomer(custId) {
    window.location.href = "/Company/CustomerAdd?Wxzh=" + Wxz + "&Id=" + custId;

}

//新增服务
function addfuwu(custId) {
    window.location.href = "/Company/CustFuWuAdd?Wxzh=" + Wxz + "&compId=" + custId;

}

//删除客户
function delcustomer(custId) {

    var $url = woowx.constant.APIBaseURL + "/api/company/DeleteCustomer";
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
