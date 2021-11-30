/***
*审批流程相关
***/
layui.define(['table', 'form'], function (exports) {
    var $ = layui.$
        , admin = layui.admin
        , setter = layui.setter
       
         , table = layui.table;
    var appflowutility = {
           /// <summary>如果没有审批隐藏审批列</summary>  
            /// <param name="htcols" type="obj">表格列</param>
        SeCols: function (pam) {
            if (setter.sysinfo.lhvs != "SE") {
            var delcols = ["WfStateDic", "WfCurrNodeName", "WfItemDic"];
            for (var i = 0; i < delcols.length; i++) {
                for (var j = 0; j < pam.htcols.length; j++) {
                    if (pam.htcols[j].field == delcols[i]) {
                        pam.htcols.splice(j, 1);

                    }
                }
                }
            }
        },
        getFlowTemp: function (param) {
            /// <summary>根据条件获取流程模板</summary>  
            /// <param name="flowItem" type="int">审批事项</param>
            /// <param name="deptId" type="int">组织机构ID</param>
            /// <param name="objType" type="int">审批对象（客户、合同....Id）</param>
            /// <param name="objCateId" type="int">审批对象类别</param>
            var checkStatus = table.checkStatus(param.tableId)
             , checkData = checkStatus.data; //得到选中的数据
            var flowitem = $(param.evtobj).attr("flowitem");
            //var flowtempId = 0;
            var $data;
            admin.req({
                url: '/WorkFlow/FlowTemp/GetFlowTemp'
            , async: false//取消异步
            , data: {
                FlowItem: flowitem,
                DeptId: param.deptId,
                ObjType: param.objType,
                ObjCateId: param.objCateId,
                ObjId: checkData[0].Id

            }
            , done: function (res) {
                $data = res.Data;
            }
            });
            return $data;
        },
        showFlow: function (param) {
            //debugger;
            var checkStatus = table.checkStatus(param.tableId)
            var checkData = checkStatus.data;
            var tempdata = appflowutility.getFlowTemp({
                tableId: param.tableId
                , evtobj: param.evtobj
                , deptId: param.deptId
                , objType: param.objType
                , objCateId: param.objCateId
            });
            if (tempdata.InstId !== 0) {
                return layer.alert("流程已经提交！");
            }
            else if (tempdata.TempId === 0) {
                return -1;
            } else {
                var opurl = "/WorkFlow/AppInst/ShowFlow?tempId="
                + tempdata.TempId + "&ftitle=" + encodeURI(encodeURI(param.objName))
                + "&ftype=" + param.objType + "&famt=" + param.objamt;
                var $title = param.objName + "--提交流程"
                layer.open({
                    type: 2
               , title: $title
               , content: opurl
               , maxmin: true
                    // , area: ['60%', '80%']
                    , btn: ['提交流程', '取消']
               , btnAlign: 'c'
               , skin: "layer-ext-myskin"
                    , yes: function (index, layero) {
                        var logdindextp = layer.load(0, { shade: false });
                   try {
                       
                       var flowitem = $(param.evtobj).attr("flowitem");
                       admin.req({
                           async: false,
                           url: "/WorkFlow/FlowTemp/ChekSubmitFlowData",
                           data: {
                               tempId: tempdata.TempId
                               , amount: param.objamt
                               , flowType: param.objType
                           },
                           type: 'POST',
                           done: function (res) {
                               var ert = setter.LCMB.Fgld;
                               // debugger;
                               //    if (res.Msg != "提交异常！" || res.Msg != "没有开始，结束节点！" || res.Msg != "没有完整的流程，可能是金额不匹配！"
                               //        || res.Msg != "没有节点信息或者节点图"|| res.Msg != "没有设置任何节点条件")
                               //    {
                               //        layer.close(logdindextp);
                               //       layer.alert("请补全" + res.Msg + "节点信息")
                               //        return layer.close(logdindextp);
                               //    } else if (res.Msg != "") {
                               //    return layer.alert(res.Msg);
                               //} else {
                               //    admin.req({
                               //        url: '/WorkFlow/AppInst/SubmitWorkFlow'
                               //        , data: {
                               //            ObjType: param.objType//审批对象类型（客户，合同。。）
                               //            , AppObjId: checkData[0].Id//对象ID
                               //            , AppObjName: param.objName//名称
                               //            , AppObjNo: param.objCode//编号
                               //            , AppObjCateId: param.objCateId//类别ID
                               //            , TempId: tempdata.TempId//模板ID
                               //            , AppObjAmount: param.objamt//金额
                               //            , Mission: flowitem
                               //            , TempHistId: tempdata.TempHistId
                               //            , FinceType: param.finceType//资金性质，合同使用
                               //            , AppSecObjId: param.AppSecObjId
                               //        }, done: function (res) {
                               //            layer.close(logdindextp);
                               //            layer.msg("提交成功", { icon: 6, time: 500 }, function (msgindex) {
                               //                table.reload(param.tableId, {
                               //                    where: { rand: wooutil.getRandom() }

                               //                });
                               //                layer.close(index);
                               //            });

                               //        }
                               //    });
                               //}

                               //if (res.Msg === "提交异常！" || res.Msg === "没有开始，结束节点！" || res.Msg === "没有完整的流程，可能是金额不匹配！"
                               //    || res.Msg === "没有节点信息或者节点图" || res.Msg === "请补全审批人信息" ) {
                               //    layer.close(logdindextp);
                               //}
                               if ((res.Msg != "提交异常！" || res.Msg != "没有开始，结束节点！" || res.Msg != "没有完整的流程，可能是金额不匹配！"
                                   || res.Msg != "没有节点信息或者节点图" || res.Msg != "没有设置任何节点条件") && res.Msg != "") {
                                   layer.close(logdindextp);

                                   return layer.alert("请补全 " + res.Msg + " 人员信息");
                               } else if (res.Msg != "") {
                                   layer.close(logdindextp);
                                   return layer.alert(res.Msg);
                               }

                               else if (res.Msg == "") {
                                   admin.req({
                                       url: '/WorkFlow/AppInst/SubmitWorkFlow'
                                       , data: {
                                           ObjType: param.objType//审批对象类型（客户，合同。。）
                                           , AppObjId: checkData[0].Id//对象ID
                                           , AppObjName: param.objName//名称
                                           , AppObjNo: param.objCode//编号
                                           , AppObjCateId: param.objCateId//类别ID
                                           , TempId: tempdata.TempId//模板ID
                                           , AppObjAmount: param.objamt//金额
                                           , Mission: flowitem
                                           , TempHistId: tempdata.TempHistId
                                           , FinceType: param.finceType//资金性质，合同使用
                                           , AppSecObjId: param.AppSecObjId
                                           , Fgld: setter.LCMB.Fgld
                                           , Ksyj: setter.LCMB.Ksyj
                                       }, done: function (res) {
                                           layer.close(logdindextp);
                                           layer.msg("提交成功", { icon: 6, time: 500 }, function (msgindex) {
                                               table.reload(param.tableId, {
                                                   where: { rand: wooutil.getRandom() }

                                               });
                                               layer.close(index);
                                           });

                                       }
                                   });
                               }



                           }

                       });
                   } catch (e) {
                       layer.alert("提交流程出现异常：" + message);
                       console.log(e.message);

                   } finally {
                       layer.close(logdindextp);
                   }


               },
                    success: function (layero, index) {
                        layer.full(index);
                        $(".layui-layer-btn0").css({
                            "font-size": "24px", "width": "150px",
                            "height": "36px", "font-weight": "bold ",
                            "text-align": "center", "vertical-align": "middle"
                            ,"padding-top":"8px"
                        });
                        $(".layui-layer-btn1").css({
                            "font-size": "24px",
                            "height": "36px", "text-align": "center", "vertical-align": "middle",
                            "font-weight": "bold ", "padding-top": "8px"

                        });
                       


                    }
                })
            }
        }

    };

    exports('appflowutility', appflowutility);
});
