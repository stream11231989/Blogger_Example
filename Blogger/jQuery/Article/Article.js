//畫面載入事件後
$(function () {
    //綁定修改文章按鈕點擊事件
    //因為修改文章按鈕是使用AjAX去回產生，因此採用$(document).delegae
    $(document).delegate('#EditArticleModal #editBtn', 'click', function () {
        $('#EditArticleModal form').submit();
    });

    //當按下新增留言0.2秒之後重整頁面
    $("#createMessage").click(function () {
        setTimeout(function () {
            location.reload();
        }, 200);
    });
});