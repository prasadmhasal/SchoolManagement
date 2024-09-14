$(document).ready(function () {
    GetSubjects();
  /*  GetTeachers();*/
    //    GetGuardian();
       GetClass();
});
function GetSubjects() {
    $.ajax({
        url: '/Admin/FetchSubjects',
        type: 'GET',
        dataType: 'json',
        success: function (result, staus, xhr) {
            console.log(result);
            var options = "";
            options += '<option value="">Select SubjectName</option>';

            $.each(result, function (index, entry) {
                options += "<option value='" + entry.subjectName + " ' name='SubjectId' >" + entry.subjectName + "</options>";
            });
            $('#GetSubjects').html(options);
        },
        error: function (ex) {
            alert('Failed to retrieve course names: ' + ex);
        }

    });
}
function GetClass() {
    $.ajax({
        url: '/Admin/FetchClass',
        type: 'GET',
        dataType: 'json',
        success: function (result, staus, xhr) {
            console.log(result);
            var options = "";
            options += '<option value="">Select Standard</option>';

            $.each(result, function (index, entry) {
                options += "<option value='" + entry.standard + " ' name='teacherId' >" + entry.standard + "</options>";
            });
            $('#FetchAllClasses').html(options);
        },
        error: function (ex) {
            alert('Failed to retrieve course names: ' + ex);
        }

    });
}


//function GetTeachers() {
//    $.ajax({
//        url: '/Admin/FetchTeachers',
//        type: 'Get',
//        dataType: 'json',
//        success: function (result, status, xhr) {

//            var options = '<option value="">Select Teacher</option>';
//            $.each(result, function (index, item) {
//                options += "<option value='" + item.teacherUserId + "'>" + item.firstName + " " + item.lastName + "</option>";
//            });

//            $("#FetchTeacherData").html(options);
//        },
//        error: function (xhr, status, error) {
//            console.error("Error fetching data: ", error);
//            alert("Failed to load subjects: " + xhr.status + " - " + error);
//        }

//    });
//}
