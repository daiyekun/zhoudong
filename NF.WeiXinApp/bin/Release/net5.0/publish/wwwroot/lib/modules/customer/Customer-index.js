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
    // 调试信息（如需调试可取消注释）
    // alert("page:" + spage + "，limit=" + pagesize + ",keyWord=" + $kekword + ",Wxzh=" + Wxz + ",FinanceType=" + HtType);

    $.ajax({
        type: "GET",
        url: $url,
        // 注意：这里原本传入的是 page 变量，如果 spage 是传入参数，请确认是否需要改为 spage
        data: { "page": page, "limit": pagesize, "keyWord": $kekword, "Wxzh": Wxz, "FinanceType": HtType },
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        timeout: 16000,
        beforeSend: function (xhr) {
            $('#loading').show();
        },
        success: function (rs) {
            // 兼容处理：如果后端返回的已经是对象，不需要 parse；如果是字符串才 parse
            var datas = (typeof rs === 'string') ? JSON.parse(rs) : rs;

            $('#loading').hide();

            if (!datas.data || datas.data == null) {
                $("#getmore").html("没有更多数据了");
                return false;
            }

            if (datas.data.length < pagesize) {
                flag = false; // 设置没有数据了标记
                $("#getmore").html("没有更多数据了");
            }

            var result = '';
            for (var i = 0; i < datas.data.length; i++) {
                var item = datas.data[i];

                // 构建 HTML 字符串
                // 修改重点：将三个按钮放在同一个 weui-cell__bd 中，并修复 onclick 的引号
                result += '<div class="weui-cells">'
                    + '<a class="weui-cell weui-cell_access" href="Detail?Id=' + item.Id + "&FinanceType=" + HtType + '">'
                    + '<div class="weui-cell__bd"><p>名称</p></div>'
                    + '<div class="weui-cell__ft list-title">' + item.Name + '</div>'
                    + '</a>'

                    + '<div class="weui-cell">'
                    + '<div class="weui-cell__bd"><p>编号</p></div>'
                    + '<div class="weui-cell__ft">' + item.Code + '</div>'
                    + '</div>'

                    + '<div class="weui-cell">'
                    + '<div class="weui-cell__bd"><p>联系人</p></div>'
                    + '<div class="weui-cell__ft">' + item.FirstContact + '</div>'
                    + '</div>'

                    + '<div class="weui-cell">'
                    + '<div class="weui-cell__bd"><p>联系电话</p></div>'
                    + '<div class="weui-cell__ft">' + item.FirstContactMobile + '</div>'
                    + '</div>'

                    // --- 修改后的按钮区域开始 ---
                    + '<div class="weui-cell">'
                    + '<div class="weui-cell__bd" style="width: 100%; padding: 5px 0;">'
                    + '<a href="javascript:;" onclick="upcustomer(' + item.Id + ')" class="weui-btn weui-btn_mini bg-green"><i class="icon icon-115"></i>修改</a>'
                    + '&nbsp;&nbsp;'
                    + '<a href="javascript:;" onclick="delcustomer(' + item.Id + ')" class="weui-btn weui-btn_mini bg-red"><i class="icon icon-115"></i>删除</a>'
                    + '&nbsp;&nbsp;'
                    + '<a href="javascript:;" onclick="addfuwu(' + item.Id + ')" class="weui-btn weui-btn_mini bg-blue"><i class="icon icon-115"></i>新增服务记录</a>'
                    + '</div>'
                    + '</div>'
                    // --- 修改后的按钮区域结束 ---

                    // 原代码中被注释的状态栏保持不动
                    // + '<div class="weui-cell  " >'
                    // + '<div class="weui-cell__bd">'
                    // + ' <p>状态</p>'
                    // + '</div>'
                    // + '<div class="weui-cell__ft">' + item.CstateDic + '</div>'
                    // + '</div>'

                    + '</div>'; // 结束 weui-cells
            }

            $("#rank-list").append(result);
        },
        error: function (xhr) {
            alert('ajax出错');
            $('#loading').hide();
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
