using CafeManagement.Models.ViewModels;
using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Controllers
{
    [Authorize(Roles = "Admin,Kế toán")]
    public class DinhluongController : Controller
    {
        private readonly IDinhluongService _dinhluongService;
        private readonly ILogger<DinhluongController> _logger;

        public DinhluongController(IDinhluongService dinhluongService, ILogger<DinhluongController> logger)
        {
            _dinhluongService = dinhluongService;
            _logger = logger;
        }

        // GET: Dinhluong
        public async Task<IActionResult> Index()
        {
            try
            {
                var recipes = await _dinhluongService.GetAllAsync();
                return View(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading recipes");
                TempData["Error"] = "Lỗi tải danh sách công thức";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Dinhluong/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = await _dinhluongService.BuildCreateModelAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                TempData["Error"] = "Lỗi tải form tạo công thức";
                return RedirectToAction("Index");
            }
        }

        // POST: Dinhluong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DinhluongEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var formModel = await _dinhluongService.BuildCreateModelAsync();
                model.SanphamOptions = formModel.SanphamOptions;
                model.ThanhphanOptions = formModel.ThanhphanOptions;
                return View(model);
            }

            try
            {
                await _dinhluongService.CreateAsync(model);
                TempData["Success"] = "Công thức đã được tạo thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                var formModel = await _dinhluongService.BuildCreateModelAsync();
                model.SanphamOptions = formModel.SanphamOptions;
                model.ThanhphanOptions = formModel.ThanhphanOptions;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recipe");
                TempData["Error"] = "Lỗi tạo công thức: " + ex.Message;
                var formModel = await _dinhluongService.BuildCreateModelAsync();
                model.SanphamOptions = formModel.SanphamOptions;
                model.ThanhphanOptions = formModel.ThanhphanOptions;
                return View(model);
            }
        }

        // GET: Dinhluong/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await _dinhluongService.BuildEditModelAsync(id);
                if (model == null)
                {
                    TempData["Error"] = "Không tìm thấy công thức";
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form");
                TempData["Error"] = "Lỗi tải form chỉnh sửa";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Dinhluong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DinhluongEditViewModel model)
        {
            if (id != model.Id)
            {
                TempData["Error"] = "ID không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var formModel = await _dinhluongService.BuildCreateModelAsync();
                model.SanphamOptions = formModel.SanphamOptions;
                model.ThanhphanOptions = formModel.ThanhphanOptions;
                return View(model);
            }

            try
            {
                await _dinhluongService.UpdateAsync(id, model);
                TempData["Success"] = "Công thức đã được cập nhật thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                var formModel = await _dinhluongService.BuildCreateModelAsync();
                model.SanphamOptions = formModel.SanphamOptions;
                model.ThanhphanOptions = formModel.ThanhphanOptions;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recipe");
                TempData["Error"] = "Lỗi cập nhật công thức: " + ex.Message;
                var formModel = await _dinhluongService.BuildCreateModelAsync();
                model.SanphamOptions = formModel.SanphamOptions;
                model.ThanhphanOptions = formModel.ThanhphanOptions;
                return View(model);
            }
        }

        // POST: Dinhluong/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var recipe = await _dinhluongService.GetByIdAsync(id);
                if (recipe == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy công thức" });
                }

                await _dinhluongService.DeleteAsync(id);
                return Json(new { success = true, message = "Công thức đã được xóa thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recipe");
                return Json(new { success = false, message = "Lỗi xóa công thức: " + ex.Message });
            }
        }
    }
}
