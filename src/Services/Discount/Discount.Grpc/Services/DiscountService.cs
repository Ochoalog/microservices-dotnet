﻿using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {            
            var coupon = await _repository.GetDiscount(request.ProductName);

            if(coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Product Name = {request.ProductName}"));
            }

            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var hasCreated = await _repository.CreateDiscount(_mapper.Map<Coupon>(request.Coupon));

            if (!hasCreated)
            {
                _logger.LogInformation("Discount is not created.");

                return null;
            }
            else
            {
                _logger.LogInformation("Discount is created succesfully.");
                return request.Coupon;
            }
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var hasCreated = await _repository.UpdateDiscount(_mapper.Map<Coupon>(request.Coupon));

            if (!hasCreated)
            {
                _logger.LogInformation("Discount is not updated.");

                return null;
            }
            else
            {
                _logger.LogInformation("Discount is updated succesfully.");
                return request.Coupon;
            }
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _repository.DeleteDiscount(request.ProductName);

            return new DeleteDiscountResponse
            {
                Success = deleted
            };
        }
    }
}
