CKEDITOR.replace('editor');

document.querySelector('.heading').onclick = () => {
    document.getElementById('close').style.display = 'initial';
    document.querySelector('.input').style.height = 'fit-content';
    document.querySelector('.input').style.position = 'absolute';
    document.querySelector('.input').style.zIndex = 5;
    document.querySelector('.container').style.opacity = 10 + '%';

    //document.querySelector('.heading').placeholder = 'Heading';
    //document.querySelector('.heading').style.borderBottom = '2px solid black';
}

document.getElementById('close').onclick = () => {
    document.querySelector('.input').style.height = 4.2 + 'rem';
    document.querySelector('.input').style.position = 'initial';
    document.querySelector('.input').style.zIndex = 1;
    document.querySelector('.container').style.opacity = 100 + '%';
    document.querySelector('.heading').value = '';
    CKEDITOR.instances.editor.setData('');
    //document.querySelector('.save_btn').style.display = 'initial';
    //document.querySelector('.update_btn').style.display = 'none';
    //document.querySelector('.heading').placeholder = 'Take a Note...';
    //document.querySelector('.heading').style.borderBottom = '0px solid black';
    document.getElementById('close').style.display = 'none';
}

function EditBtn(e) {
    document.getElementById('close').style.display = 'initial';
    document.querySelector('.save_btn').style.display = 'none';
    document.querySelector('.update_btn').style.display = 'initial';
    document.querySelector('.input').style.height = 'fit-content';
    document.querySelector('.input').style.position = 'absolute';
    document.querySelector('.input').style.zIndex = 5;
    document.querySelector('.container').style.opacity = 10 + '%';
    document.querySelector('.heading').placeholder = 'Heading';
    document.querySelector('.heading').style.borderBottom = '2px solid black';

    var crrItem = e.parentElement.parentElement;
    var _content_id = crrItem.children[0].getAttribute("content-id");
    var _page_id = crrItem.children[0].getAttribute("page-id");
    var _content_position = crrItem.children[0].getAttribute("content-position");
    var _title = crrItem.children[0].innerHTML;
    var _content = crrItem.children[1].innerHTML;

    //var title = document.getElementById('page-title').innerHTML;
    //var content = document.getElementById('page-content0').innerHTML;

    document.querySelector('.heading').value = _title;
    CKEDITOR.instances.editor.setData(_content);

    document.querySelector('.update_btn').onclick = () => {
        crrItem.firstElementChild.innerHTML = document.querySelector('.heading').value;
        document.querySelector('.heading').value = '';
        crrItem.firstElementChild.nextElementSibling.innerHTML = CKEDITOR.instances.editor.getData();
        CKEDITOR.instances.editor.setData('');
        document.querySelector('.save_btn').style.display = 'initial';
        document.querySelector('.update_btn').style.display = 'none';
        document.querySelector('.input').style.height = 4.2 + 'rem';
        document.querySelector('.input').style.position = 'initial';
        document.querySelector('.input').style.zIndex = 1;
        document.querySelector('.container').style.opacity = 100 + '%';
        document.querySelector('.heading').placeholder = 'Take a Note...';
        document.querySelector('.heading').style.borderBottom = '0px solid black';
        document.getElementById('close').style.display = 'none';

        AjaxUpdateContentById(e);
    }

    let objectContent = {
        content_id: _content_id,
        title: _title,
        content: _content,
        page_id: _page_id,
        position: _content_position
    };
}

//function GetListContent() {
//    $.ajax({
//        type: 'GET',
//        url: '@Url.Action("GetContentRule", "Home")',
//        dataType: "json",
//        contentType: "application/json; charset=utf-8",
//        success: function (results) {
//            document.getElementById("page-content").innerHTML = "";
//            if (results != null) {
//                for (let i = 0; i < results.length; i++) {
//                    console.log("" + results[i].content);
//                    document.getElementById("page-content").innerHTML +=
//                        `<div class="col-lg-12" id="content-item">
//                                            <div class="Event-Schedule" id="page-title" content-id="`+ results[i].content_id + `" page-id="` + results[i].page_id + `" content-position="` + results[i].position + `">
//                                                `+ results[i].title + `
//                                            </div>
//                                            <div class="Event-Content" id="page-content0">
//                                                `+ results[i].content + `
//                                            </div>
//                                            <div class="col-lg-12" id="delete_edit">
//                                                <button class="btn-sm btn-warning" onclick="EditBtn(this)">Edit</button>
//                                                <button class="btn-sm btn-primary" onclick="AjaxPinContentById(this)" >Pin</button>
//                                                <button class="btn-sm btn-danger" onclick="AjaxDeleteContentById(this)" >Delete</button>
//                                            </div>
//                                       </div>`;
//                }
//            }
//        }
//    });
//};

//function AjaxSaveListContent() {
//    var pageContent = document.getElementById('page-content');
//    let lstContent = [];
//    if (pageContent.childElementCount > 0) {
//        for (var i = 0; i < pageContent.childElementCount; i++) {
//            var classList = pageContent.children[i];
//            var _content_id = classList.children[0].getAttribute("content-id");
//            var _page_id = classList.children[0].getAttribute("page-id");
//            var _title = classList.children[0].innerHTML;
//            var _content = classList.children[1].innerHTML;

//            let objectContent = {
//                content_id: _content_id,
//                title: _title,
//                content: _content,
//                page_id: _page_id,
//                position: i
//            };
//            lstContent.push(objectContent);
//        }
//    }
//    console.log("id= " + lstContent[0].id + " title= " + lstContent[0].title + "content= " + lstContent[0].content + "position= " + lstContent[0].position);

//    $.ajax({
//        type: 'POST',
//        url: '@Url.Action("UpdateListContent", "Home")',
//        data: JSON.stringify(lstContent),
//        dataType: "json",
//        contentType: "application/json; charset=utf-8",
//        success: function (rs) {
//            if (rs.Status == 1) {
//                alert("success");
//            } else {
//                alert("fail");
//            }
//        }
//    });
//    GetListContent();
//};

//function AjaxUpdateContentById(e) {
//    var crrItem = e.parentElement.parentElement;
//    var _content_id = crrItem.children[0].getAttribute("content-id");
//    var _page_id = crrItem.children[0].getAttribute("page-id");
//    var _content_position = crrItem.children[0].getAttribute("content-position");
//    var _title = crrItem.children[0].innerHTML;
//    var _content = crrItem.children[1].innerHTML;

//    let objectContent = {
//        content_id: _content_id,
//        title: _title,
//        content: _content,
//        page_id: _page_id,
//        position: _content_position
//    };

//    console.log("id= " + objectContent.id + " title= " + objectContent.title + "content= " + objectContent.content);

//    $.ajax({
//        type: 'POST',
//        url: '@Url.Action("UpdateSingleContent", "Home")',
//        data: JSON.stringify(objectContent),
//        dataType: "json",
//        contentType: "application/json; charset=utf-8",
//        success: function (rs) {
//            if (rs.Status == 1) {
//                alert("success");
//            } else {
//                alert("fail");
//            }
//        }
//    });
//    GetListContent();
//};

//function AjaxDeleteContentById(e) {

//    if (confirm("DELETE this Content ???") == true) {
//        var crrItem = e.parentElement.parentElement;
//        var _content_id = crrItem.children[0].getAttribute("content-id");
//        var _page_id = crrItem.children[0].getAttribute("page-id");
//        var _content_position = crrItem.children[0].getAttribute("content-position");
//        var _title = crrItem.children[0].innerHTML;
//        var _content = crrItem.children[1].innerHTML;

//        let objectContent = {
//            content_id: _content_id,
//            title: _title,
//            content: _content,
//            page_id: _page_id,
//            position: _content_position
//        };

//        console.log("id= " + objectContent.id + " title= " + objectContent.title + "content= " + objectContent.content);

//        $.ajax({
//            type: 'POST',
//            url: '@Url.Action("DeleteSingleContent", "Home")',
//            data: JSON.stringify(objectContent),
//            dataType: "json",
//            contentType: "application/json; charset=utf-8",
//            success: function (rs) {
//                var s = rs;
//                if (rs.Status == 1) {
//                    alert("success");
//                } else {
//                    alert("fail");
//                }
//            }
//        });
//        GetListContent();
//    }
//};

//function AjaxPinContentById(e) {
//    //step 1: get all content list and sort
//    //step 2: check if id == content-id => add the first
//    //step 3: add all content in list

//    if (confirm("PIN this Content ???") == true) {
//        var crrItem = e.parentElement.parentElement;
//        var _content_id = crrItem.children[0].getAttribute("content-id");
//        var _content_position = crrItem.children[0].getAttribute("content-position");
//        var _page_id = crrItem.children[0].getAttribute("page-id");
//        var _title = crrItem.children[0].innerHTML;
//        var _content = crrItem.children[1].innerHTML;

//        let objectContent = {
//            content_id: _content_id,
//            title: _title,
//            content: _content,
//            page_id: _page_id,
//            position: _content_position
//        };

//        console.log("id= " + objectContent.id + " title= " + objectContent.title + "content= " + objectContent.content);

//        $.ajax({
//            type: 'POST',
//            url: '@Url.Action("PinPageContent", "Home")',
//            data: JSON.stringify(objectContent),
//            dataType: "json",
//            contentType: "application/json; charset=utf-8",
//            success: function (rs) {
//                var s = rs;
//                if (rs.Status == 1) {
//                    alert("success");
//                } else {
//                    alert("fail");
//                }
//            }
//        });
//        GetListContent();
//    }
//};
